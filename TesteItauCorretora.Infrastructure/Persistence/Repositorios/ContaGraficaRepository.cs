using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Core.Gateway;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Infrastructure.Persistence;

public class ContaGraficaRepository : IContaGraficaGateway
{
    private readonly AppDbContext _context;

    public ContaGraficaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(ContaGrafica conta)
    {
        await _context.ContasGraficas.AddAsync(conta);
        await _context.SaveChangesAsync();
    }

    public async Task<ContaGrafica?> ObterPorId(int id)
    {
        return await _context.ContasGraficas
            .Include(c => c.Custodias)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ContaGrafica?> ObterPorClienteId(int clienteId)
    {
        return await _context.ContasGraficas
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }
}