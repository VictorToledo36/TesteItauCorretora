using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TesteItauCorretora.Domain.Entities;
public class ItemCesta
{
    [Key]
    public int Id { get; set; }

    public int CestaRecomendacaoId { get; set; }

    [ForeignKey(nameof(CestaRecomendacaoId))]
    public CestaRecomendacao? CestaRecomendacao { get; set; }

    [Required]
    [Column(TypeName = "varchar(10)")]
    public string Ticker { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public decimal Percentual { get; set; }
}