using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("Custodias")]
    public class Custodia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContaGraficaId { get; set; }

        [ForeignKey(nameof(ContaGraficaId))]
        public ContaGrafica? ContaGrafica { get; set; }

        [Column(TypeName = "varchar(10)")]
        [Required]
        public string Ticker { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoMedio { get; set; }

        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
