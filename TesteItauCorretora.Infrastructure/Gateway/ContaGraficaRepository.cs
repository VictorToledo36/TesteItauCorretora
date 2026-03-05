using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class ContaGraficaRepository : IContaGraficaRepository
{
    private readonly AppDbContext _context;

    public ContaGraficaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContaGrafica?> ObterContaMasterAsync()
    {
        return await _context.ContasGraficas
            .FirstOrDefaultAsync(c => c.Tipo == TipoConta.Master);
    }

    public async Task<ContaGrafica?> ObterPorIdAsync(int id)
    {
        return await _context.ContasGraficas
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AdicionarAsync(ContaGrafica conta)
    {
        await _context.ContasGraficas.AddAsync(conta);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GerarProximoNumeroContaAsync(TipoConta tipo)
    {
        var prefixo = tipo == TipoConta.Master ? "MST" : "FLH";
        
        var ultimaConta = await _context.ContasGraficas
            .Where(c => c.Tipo == tipo)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        var proximoNumero = ultimaConta != null ? ultimaConta.Id + 1 : 1;
        
        return $"{prefixo}-{proximoNumero:D6}";
    }
}
