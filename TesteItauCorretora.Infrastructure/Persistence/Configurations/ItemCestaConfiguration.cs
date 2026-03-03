using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class ItemCestaConfiguration : IEntityTypeConfiguration<ItemCesta>
    {
        public void Configure(EntityTypeBuilder<ItemCesta> builder)
        {
            builder.ToTable("ItensCesta");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.CestaRecomendacaoId).IsRequired();
            builder.Property(i => i.Ticker).IsRequired().HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(i => i.Percentual).HasColumnType("decimal(5,2)").IsRequired();
            builder.HasIndex(i => i.CestaRecomendacaoId);
        }
    }
}