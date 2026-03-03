using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("Distribuicoes")]
    public class Distribuicao
    {
        [Key]
        public int Id { get; set; }

        public int OrdemCompraId { get; set; }

        [ForeignKey(nameof(OrdemCompraId))]
        public OrdemCompra? OrdemCompra { get; set; }

        public int CustodiaId { get; set; }

        [ForeignKey(nameof(CustodiaFilhoteId))]
        public Custodia? CustodiaFilhoteId { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Ticker { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoUnitario { get; set; }

        public DateTime DataDistribuicao { get; set; }
    }
}