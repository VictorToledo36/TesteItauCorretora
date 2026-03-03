using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataAdesao { get; set; }
    public ContaGrafica ContaGrafica { get; set; }
}




