namespace TesteItauCorretora.Domain.Gateway;

public interface ICotacaoRepository
{
    Task<Entities.Cotacao?> ObterUltimaCotacaoAsync(string ticker);
    Task<Dictionary<string, Entities.Cotacao>> ObterUltimasCotacoesAsync(IEnumerable<string> tickers);
}
