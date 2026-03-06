using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Core.UseCase.Clientes;
public class ConsultarCarteiraUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaRepository _contaRepository;
    private readonly ICustodiaRepository _custodiaRepository;
    private readonly ICotacaoRepository _cotacaoRepository;

    public ConsultarCarteiraUseCase(
        IClienteRepository clienteRepository,
        IContaGraficaRepository contaRepository,
        ICustodiaRepository custodiaRepository,
        ICotacaoRepository cotacaoRepository)
    {
        _clienteRepository = clienteRepository;
        _contaRepository = contaRepository;
        _custodiaRepository = custodiaRepository;
        _cotacaoRepository = cotacaoRepository;
    }

    public async Task<CarteiraResponse> Execute(int clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        var conta = await _contaRepository.ObterPorClienteIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        var custodias = await _custodiaRepository.ObterCustodiasPorContaAsync(conta.Id);

        var ativos = new List<AtivoCarteiraDto>();
        decimal valorTotalInvestido = 0;
        decimal valorAtualCarteira = 0;

        foreach (var custodia in custodias)
        {
            var cotacao = await _cotacaoRepository.ObterUltimaCotacaoAsync(custodia.Ticker);
            var cotacaoAtual = cotacao?.PrecoFechamento ?? custodia.PrecoMedio;

            var valorInvestido = custodia.Quantidade * custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * cotacaoAtual;
            var pl = valorAtual - valorInvestido;
            var plPercentual = valorInvestido > 0 ? (pl / valorInvestido) * 100 : 0;

            valorTotalInvestido += valorInvestido;
            valorAtualCarteira += valorAtual;

            ativos.Add(new AtivoCarteiraDto
            {
                Ticker = custodia.Ticker,
                Quantidade = custodia.Quantidade,
                PrecoMedio = custodia.PrecoMedio,
                CotacaoAtual = cotacaoAtual,
                ValorAtual = valorAtual,
                Pl = pl,
                PlPercentual = Math.Round(plPercentual, 2)
            });
        }

        foreach (var ativo in ativos)
            ativo.ComposicaoCarteira = valorAtualCarteira > 0
                ? Math.Round((ativo.ValorAtual / valorAtualCarteira) * 100, 2)
                : 0;

        var plTotal = valorAtualCarteira - valorTotalInvestido;
        var rentabilidade = valorTotalInvestido > 0
            ? Math.Round((plTotal / valorTotalInvestido) * 100, 2)
            : 0;

        return new CarteiraResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            ContaGrafica = conta.NumeroConta,
            DataConsulta = DateTime.Now,
            Resumo = new ResumoCarteiraDto
            {
                ValorTotalInvestido = valorTotalInvestido,
                ValorAtualCarteira = valorAtualCarteira,
                PlTotal = plTotal,
                RentabilidadePercentual = rentabilidade
            },
            Ativos = ativos
        };
    }
}