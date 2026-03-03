using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.CPF).IsUnique();
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(200).HasColumnType("varchar(200)");
            builder.Property(c => c.CPF).IsRequired().HasMaxLength(11).HasColumnType("varchar(11)");
            builder.Property(c => c.Email).IsRequired().HasMaxLength(200).HasColumnType("varchar(200)");
            builder.Property(c => c.ValorMensal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.Ativo).IsRequired().HasDefaultValue(true);
            builder.Property(c => c.DataAdesao).IsRequired();
        }
    }
}