namespace TesteItauCorretora.Domain.Gateway;

public interface ICustodiaRepository
{
    Task<Entities.Custodia?> ObterCustodiaMasterPorTickerAsync(string ticker);
    Task<IEnumerable<Entities.Custodia>> ObterCustodiasMasterAsync();
    Task<Entities.Custodia?> ObterCustodiaFilhotePorContaAsync(int contaGraficaId, string ticker);
    Task AdicionarAsync(Entities.Custodia custodia);
    Task AtualizarAsync(Entities.Custodia custodia);
}
