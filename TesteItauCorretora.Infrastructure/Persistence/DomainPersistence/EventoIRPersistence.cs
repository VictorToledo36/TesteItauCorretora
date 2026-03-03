using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("EventosIR")]
    public class EventoIR
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        public TipoEventoIR Tipo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorBase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorIR { get; set; }

        public bool PublicadoKafka { get; set; }

        public DateTime DataEvento { get; set; }
    }
}