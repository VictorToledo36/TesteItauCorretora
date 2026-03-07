using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TesteItauCorretora.Domain.UseCase;
using TesteItauCorretora.Infrastructure.Configuration;

namespace TesteItauCorretora.Infrastructure.BackgroundServices;

public class MotorCompraBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MotorCompraBackgroundService> _logger;
    private readonly CronExpression _schedule;
    private readonly bool _isEnabled;
    private readonly string _cronExpression;
    private DateTime _nextRun;

    // Dias de execução conforme RN-020
    private static readonly int[] DiasDeExecucao = { 5, 15, 25 };

    public MotorCompraBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MotorCompraBackgroundService> logger,
        IOptions<MotorCompraConfig> config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var configValue = config.Value;
        _isEnabled = configValue.Enabled;
        _cronExpression = configValue.CronExpression;

        _schedule = CronExpression.Parse(_cronExpression, CronFormat.IncludeSeconds);
        _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow) ?? DateTime.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isEnabled)
        {
            _logger.LogInformation("[MotorCompra] Background Service está desabilitado.");
            return;
        }

        _logger.LogInformation(
            "[MotorCompra] Background Service iniciado com cron: {CronExpression}",
            _cronExpression);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            if (now >= _nextRun)
            {
                var horarioBrasilia = TimeZoneInfo.ConvertTimeFromUtc(now,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

               // var horarioBrasilia = new DateTime(2026, 3, 5, 10, 0, 0);  //Forçar dia 05 para testar o serviço

                if (EhDiaDeExecucao(horarioBrasilia))
                {
                    try
                    {
                        _logger.LogInformation(
                            "[MotorCompra] Iniciando execução para o dia {Data}",
                            horarioBrasilia.ToString("dd/MM/yyyy"));

                        using var scope = _serviceProvider.CreateScope();
                        var motorCompra = scope.ServiceProvider.GetRequiredService<MotorCompra>();

                        var resultado = await motorCompra.ExecutarCompraAsync(horarioBrasilia);

                        if (resultado.Sucesso)
                        {
                            _logger.LogInformation(
                                "[MotorCompra] Execução concluída com sucesso. " +
                                "Total aportado: {ValorTotal:C} | Ordens: {Ordens} | Clientes: {Clientes}",
                                resultado.ValorTotalAportes,
                                resultado.OrdensExecutadas.Count,
                                resultado.DistribuicoesPorCliente.Count);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "[MotorCompra] Execução finalizada com aviso: {Mensagem}",
                                resultado.Mensagem);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "[MotorCompra] Erro durante a execução em {Data}",
                            now);
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "[MotorCompra] Dia {Dia} não é dia de execução (5, 15 ou 25). Aguardando próxima janela.",
                        horarioBrasilia.Day);
                }

                _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow) ?? DateTime.UtcNow.AddSeconds(5);
            }

            var delay = GetDelayUntilNextRun();
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("[MotorCompra] Background Service finalizado.");
    }


    // Verifica se a data é um dia de execução (5, 15 ou 25) conforme RN-020.
    // Se cair no fim de semana, o próprio MotorCompra ajusta para o próximo dia útil (RN-021).
    private bool EhDiaDeExecucao(DateTime data)
    {
        return DiasDeExecucao.Contains(data.Day);
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        var now = DateTime.UtcNow;
        var timeUntilNextRun = _nextRun - now;

        if (timeUntilNextRun <= TimeSpan.Zero)
            return TimeSpan.FromMilliseconds(1000);

        // Verifica no máximo a cada 5 minutos
        if (timeUntilNextRun > TimeSpan.FromMinutes(5))
            return TimeSpan.FromMinutes(5);

        return timeUntilNextRun;
    }
}