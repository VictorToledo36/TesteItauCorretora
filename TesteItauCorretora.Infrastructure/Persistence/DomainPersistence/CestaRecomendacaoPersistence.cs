using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Table("CestasRecomendacoes")]
    public class CestaRecomendacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Nome { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;

        public DateTime DataCriacao { get; set; }

        public DateTime? DataDesativacao { get; set; }

        public ICollection<ItemCestaPersistence>? Itens { get; set; }
    }
}