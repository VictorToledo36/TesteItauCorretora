using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class Distribuicao
{
    public int Id { get; set; }
    public int OrdemCompraId { get; set; }
    public int CustodiaFilhoteId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public DateTime DataDistribuicao { get; set; }
}
