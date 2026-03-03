using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class DistribuicaoConfiguration : IEntityTypeConfiguration<Distribuicao>
    {
        public void Configure(EntityTypeBuilder<Distribuicao> builder)
        {
            builder.ToTable("Distribuicoes");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.OrdemCompraId).IsRequired();
            builder.Property(d => d.CustodiaFilhoteId).IsRequired();
            builder.Property(d => d.Ticker).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(d => d.Quantidade).IsRequired();
            builder.Property(d => d.PrecoUnitario).HasColumnType("decimal(18,4)");
            builder.Property(d => d.DataDistribuicao).IsRequired();
            builder.HasIndex(d => new { d.CustodiaFilhoteId, d.Ticker }).IsUnique();
            builder.HasIndex(d => d.OrdemCompraId);
        }
    }
}