using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class CustodiaConfiguration : IEntityTypeConfiguration<Custodia>
    {
        public void Configure(EntityTypeBuilder<Custodia> builder)
        {
            builder.ToTable("Custodias");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ContaGraficaId).IsRequired();
            builder.Property(c => c.Ticker).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(c => c.Quantidade).IsRequired();
            builder.Property(c => c.PrecoMedio).HasColumnType("decimal(18,4)");
            builder.Property(c => c.DataUltimaAtualizacao).IsRequired();
            builder.HasIndex(c => new { c.ContaGraficaId, c.Ticker }).IsUnique();
        }
    }
}