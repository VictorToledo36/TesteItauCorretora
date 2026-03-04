using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class ContaGraficaConfiguration : IEntityTypeConfiguration<ContaGrafica>
    {
        public void Configure(EntityTypeBuilder<ContaGrafica> builder)
        {
            builder.ToTable("ContasGraficas");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.NumeroConta).IsUnique();
            builder.Property(c => c.NumeroConta).IsRequired().HasMaxLength(20).HasColumnType("varchar(20)");
            builder.Property(c => c.Tipo).IsRequired().HasConversion<int>();
            builder.Property(c => c.DataCriacao).IsRequired();
            builder.HasMany(c => c.Custodias).WithOne(c => c.ContaGrafica).HasForeignKey(c => c.ContaGraficaId);

        }
    }
}