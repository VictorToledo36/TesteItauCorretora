using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("Cotacoes")]
    public class Cotacao
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataPregao { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Ticker { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoAbertura { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoFechamento { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoMaximo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoMinimo { get; set; }
    }
}