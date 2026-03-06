namespace TesteItauCorretora.Domain.Gateway;

public interface ICestaRecomendacaoRepository
{
    Task<Entities.CestaRecomendacao?> ObterCestaAtivaAsync();
    Task<IEnumerable<Entities.CestaRecomendacao>> ObterHistoricoAsync();
    Task AdicionarAsync(Entities.CestaRecomendacao cesta);
    Task AtualizarAsync(Entities.CestaRecomendacao cesta);
}
