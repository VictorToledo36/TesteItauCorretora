using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class CestaRecomendacaoConfiguration : IEntityTypeConfiguration<CestaRecomendacao>
    {
        public void Configure(EntityTypeBuilder<CestaRecomendacao> builder)
        {
            builder.ToTable("CestasRecomendacoes");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(c => c.Ativa).IsRequired();
            builder.Property(c => c.DataCriacao).IsRequired();
            builder.Property(c => c.DataDesativacao);
            builder.HasMany(c => c.Itens).WithOne() .HasForeignKey(i => i.CestaRecomendacaoId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}