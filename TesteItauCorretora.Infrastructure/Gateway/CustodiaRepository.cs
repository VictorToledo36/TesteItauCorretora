using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class CustodiaRepository : ICustodiaRepository
{
    private readonly AppDbContext _context;

    public CustodiaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Custodia?> ObterCustodiaMasterPorTickerAsync(string ticker)
    {
        return await _context.Custodias
            .Include(c => c.ContaGrafica)
            .FirstOrDefaultAsync(c => c.Ticker == ticker && c.ContaGrafica.Tipo == TipoConta.Master);
    }

    public async Task<IEnumerable<Custodia>> ObterCustodiasMasterAsync()
    {
        return await _context.Custodias
            .Include(c => c.ContaGrafica)
            .Where(c => c.ContaGrafica.Tipo == TipoConta.Master)
            .ToListAsync();
    }

    public async Task<Custodia?> ObterCustodiaFilhotePorContaAsync(int contaGraficaId, string ticker)
    {
        return await _context.Custodias
            .FirstOrDefaultAsync(c => c.ContaGraficaId == contaGraficaId && c.Ticker == ticker);
    }

    public async Task AdicionarAsync(Custodia custodia)
    {
        await _context.Custodias.AddAsync(custodia);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Custodia custodia)
    {
        _context.Custodias.Update(custodia);
        await _context.SaveChangesAsync();
    }
}
