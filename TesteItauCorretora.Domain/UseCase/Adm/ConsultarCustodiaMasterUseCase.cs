// TesteItauCorretora.Domain/UseCases/ConsultarCustodiaMasterUseCase.cs
using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Domain.UseCases;

public class ConsultarCustodiaMasterUseCase
{
    private readonly IContaGraficaRepository _contaRepository;
    private readonly ICustodiaRepository _custodiaRepository;
    private readonly ICotacaoRepository _cotacaoRepository;

    public ConsultarCustodiaMasterUseCase(
        IContaGraficaRepository contaRepository,
        ICustodiaRepository custodiaRepository,
        ICotacaoRepository cotacaoRepository)
    {
        _contaRepository = contaRepository;
        _custodiaRepository = custodiaRepository;
        _cotacaoRepository = cotacaoRepository;
    }

    public async Task<CustodiaMasterResponse> ExecutarAsync()
    {
        var contaMaster = await _contaRepository.ObterContaMasterAsync()
            ?? throw new InvalidOperationException("Conta master não encontrada.");

        var custodias = await _custodiaRepository.ObterCustodiasMasterAsync();

        var itens = new List<ItemCustodiaMasterDto>();
        decimal valorTotalResiduo = 0;

        foreach (var custodia in custodias)
        {
            if (custodia.Quantidade == 0) continue;

            var cotacao = await _cotacaoRepository.ObterUltimaCotacaoAsync(custodia.Ticker);
            var precoAtual = cotacao?.PrecoFechamento ?? custodia.PrecoMedio;
            var valorAtual = custodia.Quantidade * precoAtual;

            valorTotalResiduo += valorAtual;

            itens.Add(new ItemCustodiaMasterDto
            {
                Ticker = custodia.Ticker,
                Quantidade = custodia.Quantidade,
                PrecoMedio = custodia.PrecoMedio,
                ValorAtual = valorAtual,
                Origem = $"Residuo distribuicao {custodia.DataUltimaAtualizacao:yyyy-MM-dd}"
            });
        }

        return new CustodiaMasterResponse
        {
            ContaMaster = new ContaMasterDto
            {
                Id = contaMaster.Id,
                NumeroConta = contaMaster.NumeroConta,
                Tipo = contaMaster.Tipo.ToString().ToUpper()
            },
            Custodia = itens,
            ValorTotalResiduo = valorTotalResiduo
        };
    }
}