using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TesteItauCorretora.Domain.DTOs;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Configuration;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class EventoIRPublisher : IEventoIRPublisher, IDisposable
{
    private readonly KafkaSettings _settings;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<EventoIRPublisher> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public EventoIRPublisher(IOptions<KafkaSettings> settings, ILogger<EventoIRPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,

            // Autenticação Confluent Cloud
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _settings.ApiKey,
            SaslPassword = _settings.ApiSecret,

            // Confiabilidade
            Acks = Acks.All,
            MessageTimeoutMs = 10000,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public Task PublicarAsync(EventoIR eventoIR, string cpf)
        => PublicarLoteAsync(new List<(EventoIR, string)> { (eventoIR, cpf) });

    public async Task PublicarLoteAsync(IEnumerable<(EventoIR Evento, string CPF)> eventos)
    {
        var listaEventos = eventos.ToList();

        if (!listaEventos.Any())
        {
            _logger.LogWarning("[Kafka] Nenhum evento de IR para publicar.");
            return;
        }

        _logger.LogInformation("[Kafka] Publicando lote de {Total} evento(s) de IR...", listaEventos.Count);

        // Cria uma Task para cada evento e executa todas em paralelo
        var tasks = listaEventos.Select(item => PublicarUmAsync(item.Evento, item.CPF));

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("[Kafka] Lote publicado com sucesso. {Total} evento(s) enviados.", listaEventos.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError("[Kafka] Erro ao publicar lote de IR: {Erro}", ex.Message);
            throw;
        }
    }

    private async Task PublicarUmAsync(EventoIR eventoIR, string cpf)
    {
        var mensagem = new EventoIRMensagem
        {
            ClienteId = eventoIR.ClienteId,
            CPF = cpf,
            Ticker = eventoIR.Ticker,
            Quantidade = eventoIR.Quantidade,
            ValorOperacao = eventoIR.ValorOperacao,
            ValorIR = eventoIR.ValorIR,
            TipoEvento = eventoIR.TipoEvento.ToString(),
            DataEvento = eventoIR.DataEvento
        };

        var payload = JsonSerializer.Serialize(mensagem, _jsonOptions);
        var chave = $"{eventoIR.ClienteId}-{eventoIR.Ticker}-{eventoIR.DataEvento:yyyyMMdd}";

        try
        {
            var result = await _producer.ProduceAsync(
                _settings.TopicoIR,
                new Message<string, string>
                {
                    Key = chave,
                    Value = payload
                });

            _logger.LogInformation(
                "[Kafka] IR publicado | ClienteId: {ClienteId} | Ticker: {Ticker} | ValorIR: {ValorIR:C} | Offset: {Offset}",
                eventoIR.ClienteId, eventoIR.Ticker, eventoIR.ValorIR, result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(
                "[Kafka] ERRO ao publicar IR | ClienteId: {ClienteId} | Ticker: {Ticker} | Erro: {Erro}",
                eventoIR.ClienteId, eventoIR.Ticker, ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(5));
        _producer?.Dispose();
    }
}