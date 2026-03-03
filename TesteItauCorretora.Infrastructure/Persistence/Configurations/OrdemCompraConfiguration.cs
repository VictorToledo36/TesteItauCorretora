using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class OrdemCompraConfiguration : IEntityTypeConfiguration<OrdemCompra>
    {
        public void Configure(EntityTypeBuilder<OrdemCompra> builder)
        {
            builder.ToTable("OrdensCompras");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.ContaMasterId).IsRequired();
            builder.Property(o => o.Ticker).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(o => o.Quantidade).IsRequired();
            builder.Property(o => o.PrecoUnitario).HasColumnType("decimal(18,4)").IsRequired();
            builder.Property(o => o.TipoMercado).IsRequired();
            builder.Property(o => o.DataExecucao).IsRequired();
            builder.HasOne<ContaGrafica>().WithMany().HasForeignKey(o => o.ContaMasterId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}