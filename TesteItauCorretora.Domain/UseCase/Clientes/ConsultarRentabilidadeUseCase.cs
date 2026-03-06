using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Core.UseCase.Clientes;
public class ConsultarRentabilidadeUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaRepository _contaRepository;
    private readonly ICustodiaRepository _custodiaRepository;
    private readonly IDistribuicaoRepository _distribuicaoRepository;
    private readonly ICotacaoRepository _cotacaoRepository;

    public ConsultarRentabilidadeUseCase(
        IClienteRepository clienteRepository,
        IContaGraficaRepository contaRepository,
        ICustodiaRepository custodiaRepository,
        IDistribuicaoRepository distribuicaoRepository,
        ICotacaoRepository cotacaoRepository)
    {
        _clienteRepository = clienteRepository;
        _contaRepository = contaRepository;
        _custodiaRepository = custodiaRepository;
        _distribuicaoRepository = distribuicaoRepository;
        _cotacaoRepository = cotacaoRepository;
    }

    public async Task<RentabilidadeResponse> Execute(int clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        var conta = await _contaRepository.ObterPorClienteIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        var custodias = await _custodiaRepository.ObterCustodiasPorContaAsync(conta.Id);
        var distribuicoes = await _distribuicaoRepository.ObterPorContaAsync(conta.Id);

        var aportesAgrupados = distribuicoes
            .GroupBy(d => d.DataDistribuicao.Date)
            .OrderBy(g => g.Key)
            .ToList();

        var historicoAportes = new List<HistoricoAporteDto>();
        var totalParcelas = aportesAgrupados.Count;

        for (int i = 0; i < aportesAgrupados.Count; i++)
        {
            var grupo = aportesAgrupados[i];
            var valorAporte = grupo.Sum(d => d.Quantidade * d.PrecoUnitario);
            historicoAportes.Add(new HistoricoAporteDto
            {
                Data = grupo.Key,
                Valor = valorAporte,
                Parcela = $"{i + 1}/{totalParcelas}"
            });
        }

        var evolucaoCarteira = new List<EvolucaoCarteiraDto>();
        decimal valorInvestidoAcumulado = 0;

        foreach (var grupo in aportesAgrupados)
        {
            valorInvestidoAcumulado += grupo.Sum(d => d.Quantidade * d.PrecoUnitario);

            var tickers = custodias.Select(c => c.Ticker).Distinct();
            decimal valorCarteiraNaData = 0;

            foreach (var ticker in tickers)
            {
                var custodia = custodias.FirstOrDefault(c => c.Ticker == ticker);
                if (custodia == null) continue;

                var cotacao = await _cotacaoRepository.ObterUltimaCotacaoAsync(ticker);
                var preco = cotacao?.PrecoFechamento ?? custodia.PrecoMedio;
                valorCarteiraNaData += custodia.Quantidade * preco;
            }

            var rentabilidade = valorInvestidoAcumulado > 0
                ? Math.Round(((valorCarteiraNaData - valorInvestidoAcumulado) / valorInvestidoAcumulado) * 100, 2)
                : 0;

            evolucaoCarteira.Add(new EvolucaoCarteiraDto
            {
                Data = grupo.Key,
                ValorCarteira = valorCarteiraNaData,
                ValorInvestido = valorInvestidoAcumulado,
                Rentabilidade = rentabilidade
            });
        }

        decimal valorTotalInvestido = custodias.Sum(c => c.Quantidade * c.PrecoMedio);
        decimal valorAtualCarteira = 0;

        foreach (var custodia in custodias)
        {
            var cotacao = await _cotacaoRepository.ObterUltimaCotacaoAsync(custodia.Ticker);
            var preco = cotacao?.PrecoFechamento ?? custodia.PrecoMedio;
            valorAtualCarteira += custodia.Quantidade * preco;
        }

        var plTotal = valorAtualCarteira - valorTotalInvestido;
        var rentabilidadePercentual = valorTotalInvestido > 0
            ? Math.Round((plTotal / valorTotalInvestido) * 100, 2)
            : 0;

        return new RentabilidadeResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            DataConsulta = DateTime.Now,
            Rentabilidade = new RentabilidadeResumoDto
            {
                ValorTotalInvestido = valorTotalInvestido,
                ValorAtualCarteira = valorAtualCarteira,
                PlTotal = plTotal,
                RentabilidadePercentual = rentabilidadePercentual
            },
            HistoricoAportes = historicoAportes,
            EvolucaoCarteira = evolucaoCarteira
        };
    }
}