using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.Configurations
{
    public class EventoIRConfiguration : IEntityTypeConfiguration<EventoIR>
    {
        public void Configure(EntityTypeBuilder<EventoIR> builder)
        {
            builder.ToTable("EventosIR");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ClienteId).IsRequired();
            builder.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            builder.Property(e => e.Quantidade).IsRequired();
            builder.Property(e => e.ValorOperacao).HasColumnType("decimal(18,2)");
            builder.Property(e => e.ValorIR).HasColumnType("decimal(18,6)");
            builder.Property(e => e.TipoEvento).IsRequired();
            builder.Property(e => e.DataEvento).IsRequired();
            builder.Property(e => e.PublicadoKafka).IsRequired();
            builder.HasIndex(e => e.ClienteId);
        }
    }
}