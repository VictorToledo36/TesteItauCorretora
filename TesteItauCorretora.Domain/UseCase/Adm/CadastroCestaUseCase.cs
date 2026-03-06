using TesteItauCorretora.Domain.DTOs;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Domain.UseCase.Adm;

public class CadastrarCestaUseCase
{
    private readonly ICestaRecomendacaoRepository _repository;

    public CadastrarCestaUseCase(ICestaRecomendacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<CestaResponse> ExecutarAsync(CestaRequest request)
    {
        var itens = request.Itens.Select(i => new ItemCesta(i.Ticker, i.Percentual)).ToList();
        var novaCesta = new CestaRecomendacao(request.Nome, itens);

        var cestaAtiva = await _repository.ObterCestaAtivaAsync();
        var rebalanceamento = false;
        CestaAnteriorResponse? cestaAnterior = null;
        List<string>? ativosRemovidos = null;
        List<string>? ativosAdicionados = null;

        if (cestaAtiva != null)
        {
            var diferenca = novaCesta.CompararCom(cestaAtiva);

            cestaAtiva.Desativar();
            await _repository.AtualizarAsync(cestaAtiva);

            rebalanceamento = true;
            cestaAnterior = new CestaAnteriorResponse
            {
                CestaId = cestaAtiva.Id,
                Nome = cestaAtiva.Nome,
                DataDesativacao = cestaAtiva.DataDesativacao!.Value
            };
            ativosRemovidos = diferenca.AcoesQueSairam;
            ativosAdicionados = diferenca.AcoesQueEntraram;
        }

        await _repository.AdicionarAsync(novaCesta);

        var mensagem = rebalanceamento
            ? $"Cesta atualizada. Rebalanceamento disparado."
            : "Primeira cesta cadastrada com sucesso.";

        return new CestaResponse
        {
            CestaId = novaCesta.Id,
            Nome = novaCesta.Nome,
            Ativa = novaCesta.Ativa,
            DataCriacao = novaCesta.DataCriacao,
            Itens = request.Itens,
            RebalanceamentoDisparado = rebalanceamento,
            Mensagem = mensagem,
            CestaAnteriorDesativada = cestaAnterior,
            AtivosRemovidos = ativosRemovidos,
            AtivosAdicionados = ativosAdicionados
        };
    }
}