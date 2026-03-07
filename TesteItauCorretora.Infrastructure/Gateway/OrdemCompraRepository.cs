using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class OrdemCompraRepository : IOrdemCompraRepository
{
    private readonly AppDbContext _context;

    public OrdemCompraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(OrdemCompra ordem)
    {
        await _context.OrdensCompra.AddAsync(ordem);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrdemCompra>> ObterPorDataAsync(DateTime data)
    {
        return await _context.OrdensCompra
            .Where(o => o.DataExecucao.Date == data.Date)
            .ToListAsync();
    }
}