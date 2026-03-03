using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class CotacaoConfiguration : IEntityTypeConfiguration<Cotacao>
    {
        public void Configure(EntityTypeBuilder<Cotacao> builder)
        {
            builder.ToTable("Cotacoes");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.DataPregao).IsRequired();
            builder.Property(c => c.Ticker).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(c => c.PrecoAbertura).HasColumnType("decimal(18,4)");
            builder.Property(c => c.PrecoFechamento).HasColumnType("decimal(18,4)");
            builder.Property(c => c.PrecoMaximo).HasColumnType("decimal(18,4)");
            builder.Property(c => c.PrecoMinimo).HasColumnType("decimal(18,4)");
        }
    }
}