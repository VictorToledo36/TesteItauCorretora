using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteItauCorretora.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TesteItauCorretora.Infrastructure.Persistence.DomainPersistence
{
    [Index(nameof(CPF), IsUnique = true)]
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;
        [Required]
        [MaxLength(11)]
        [Column(TypeName = "varchar(11)")]
        public string CPF { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ValorMensal { get; set; }
        [Required]
        public bool Ativo { get; set; } = true;
        [Required]
        public DateTime DataAdesao { get; set; }
        [Required]
        public ContaGrafica ContaGrafica { get; set; }
    }
}
