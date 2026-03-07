using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Domain.UseCases;

public class ConsultarCestaAtualUseCase
{
    private readonly ICestaRecomendacaoRepository _cestaRepository;
    private readonly ICotacaoRepository _cotacaoRepository;

    public ConsultarCestaAtualUseCase(
        ICestaRecomendacaoRepository cestaRepository,
        ICotacaoRepository cotacaoRepository)
    {
        _cestaRepository = cestaRepository;
        _cotacaoRepository = cotacaoRepository;
    }

    public async Task<CestaAtualResponse?> ExecutarAsync()
    {
        var cesta = await _cestaRepository.ObterCestaAtivaAsync();
        if (cesta == null) return null;

        var itens = new List<ItemCestaComCotacaoDto>();
        foreach (var item in cesta.Itens)
        {
            var cotacao = await _cotacaoRepository.ObterUltimaCotacaoAsync(item.Ticker);
            itens.Add(new ItemCestaComCotacaoDto
            {
                Ticker = item.Ticker,
                Percentual = item.Percentual,
                CotacaoAtual = cotacao?.PrecoFechamento ?? 0
            });
        }

        return new CestaAtualResponse
        {
            CestaId = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            Itens = itens
        };
    }
}