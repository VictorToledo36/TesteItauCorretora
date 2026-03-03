using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Index(nameof(NumeroConta), IsUnique = true)]
    [Table("ContasGraficas")]
    public class ContaGrafica
    {
        [Key]
        public int Id { get; set; }

        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string NumeroConta { get; set; } = string.Empty;

        public TipoConta Tipo { get; set; }

        public DateTime DataCriacao { get; set; }

        public ICollection<Custodia>? Custodias { get; set; }
    }
}