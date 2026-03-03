using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("Rebalanceamentos")]
    public class Rebalanceamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        public TipoRebalanceamento Tipo { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string TickerVendido { get; set; } = string.Empty;

        [Column(TypeName = "varchar(10)")]
        public string TickerComprado { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorVenda { get; set; }

        public DateTime DataRebalanceamento { get; set; }
    }
}