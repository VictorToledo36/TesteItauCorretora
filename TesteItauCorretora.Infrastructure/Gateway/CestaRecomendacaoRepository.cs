using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class CestaRecomendacaoRepository : ICestaRecomendacaoRepository
{
    private readonly AppDbContext _context;

    public CestaRecomendacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CestaRecomendacao?> ObterCestaAtivaAsync()
    {
        return await _context.CestasRecomendacao
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Ativa);
    }

    public async Task AdicionarAsync(CestaRecomendacao cesta)
    {
        await _context.CestasRecomendacao.AddAsync(cesta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(CestaRecomendacao cesta)
    {
        _context.CestasRecomendacao.Update(cesta);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CestaRecomendacao>> ObterHistoricoAsync()
    {
        return await _context.CestasRecomendacao
            .Include(c => c.Itens)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }
}
