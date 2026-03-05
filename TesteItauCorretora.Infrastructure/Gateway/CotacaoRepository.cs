using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class CotacaoRepository : ICotacaoRepository
{
    private readonly AppDbContext _context;

    public CotacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cotacao?> ObterUltimaCotacaoAsync(string ticker)
    {
        return await _context.Cotacoes
            .Where(c => c.Ticker == ticker)
            .OrderByDescending(c => c.DataPregao)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<string, Cotacao>> ObterUltimasCotacoesAsync(IEnumerable<string> tickers)
    {
        var tickersList = tickers.ToList();
        
        var cotacoes = await _context.Cotacoes
            .Where(c => tickersList.Contains(c.Ticker))
            .GroupBy(c => c.Ticker)
            .Select(g => g.OrderByDescending(c => c.DataPregao).First())
            .ToListAsync();

        return cotacoes.ToDictionary(c => c.Ticker, c => c);
    }
}
