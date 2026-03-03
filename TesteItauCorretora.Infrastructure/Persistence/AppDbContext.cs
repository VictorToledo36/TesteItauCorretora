using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ContaGrafica> ContasGraficas { get; set; }
        public DbSet<Cotacao> Cotacoes { get; set; }
        public DbSet<Custodia> Custodias { get; set; }
        public DbSet<OrdemCompra> OrdensCompra { get; set; }
        public DbSet<CestaRecomendacao> CestasRecomendacao { get; set; }
        public DbSet<ItemCesta> ItensCesta { get; set; }
        public DbSet<Distribuicao> Distribuicoes { get; set; }
        public DbSet<EventoIR> EventosIR { get; set; }
        public DbSet<Rebalanceamento> Rebalanceamentos { get; set; }
    }
}