using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Domain.UseCases;

public class ConsultarHistoricoCestasUseCase
{
    private readonly ICestaRecomendacaoRepository _cestaRepository;

    public ConsultarHistoricoCestasUseCase(ICestaRecomendacaoRepository cestaRepository)
    {
        _cestaRepository = cestaRepository;
    }

    public async Task<HistoricoCestasResponse> ExecutarAsync()
    {
        var cestas = await _cestaRepository.ObterHistoricoAsync();

        var lista = cestas.Select(c => new CestaHistoricoDto
        {
            CestaId = c.Id,
            Nome = c.Nome,
            Ativa = c.Ativa,
            DataCriacao = c.DataCriacao,
            DataDesativacao = c.DataDesativacao,
            Itens = c.Itens.Select(i => new ItemCestaDto
            {
                Ticker = i.Ticker,
                Percentual = i.Percentual
            }).ToList()
        }).ToList();

        return new HistoricoCestasResponse { Cestas = lista };
    }
}