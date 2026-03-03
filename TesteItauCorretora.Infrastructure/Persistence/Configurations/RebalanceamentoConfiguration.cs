using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class RebalanceamentoConfiguration : IEntityTypeConfiguration<Rebalanceamento>
    {
        public void Configure(EntityTypeBuilder<Rebalanceamento> builder)
        {
            builder.ToTable("Rebalanceamentos");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ClienteId).IsRequired();
            builder.Property(r => r.TickerVendido).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(r => r.TickerComprado).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(r => r.ValorVenda).HasColumnType("decimal(18,2)");
            builder.Property(r => r.DataRebalanceamento).IsRequired();
            builder.HasIndex(r => r.ClienteId);
        }
    }
}