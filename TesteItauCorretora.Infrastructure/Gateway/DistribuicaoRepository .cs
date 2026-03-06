using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;
public class DistribuicaoRepository : IDistribuicaoRepository
{
    private readonly AppDbContext _context;

    public DistribuicaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Distribuicao>> ObterPorCustodiaAsync(int custodiaFilhoteId)
    {
        return await _context.Distribuicoes
            .Where(d => d.CustodiaFilhoteId == custodiaFilhoteId)
            .OrderBy(d => d.DataDistribuicao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Distribuicao>> ObterPorContaAsync(int contaGraficaId)
    {
        return await _context.Distribuicoes
            .Where(d => _context.Custodias
                .Any(c => c.Id == d.CustodiaFilhoteId && c.ContaGraficaId == contaGraficaId))
            .OrderBy(d => d.DataDistribuicao)
            .ToListAsync();
    }
}