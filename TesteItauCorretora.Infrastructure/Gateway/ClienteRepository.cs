using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> ObterClientesAtivosAsync()
    {
        return await _context.Clientes
            .Include(c => c.ContasGraficas)
            .Where(c => c.Ativo)
            .ToListAsync();
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.ContasGraficas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> ObterPorCpfAsync(string cpf)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.CPF == cpf);
    }

    public async Task<Cliente?> ObterPorEmailAsync(string email)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }
}
