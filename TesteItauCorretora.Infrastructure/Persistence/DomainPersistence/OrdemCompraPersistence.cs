using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("OrdensCompras")]
    public class OrdemCompra
    {
        [Key]
        public int Id { get; set; }

        public int ContaMasterId { get; set; }

        [ForeignKey(nameof(ContaMasterId))]
        public ContaGrafica? ContaMaster { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Ticker { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoUnitario { get; set; }

        public TipoMercado TipoMercado { get; set; }

        public DateTime DataExecucao { get; set; }

        public ICollection<Distribuicao>? Distribuicoes { get; set; }
    }
}