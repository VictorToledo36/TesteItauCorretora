using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public enum TipoMercado
{
    Lote = 1,
    Fracionario = 2
}

public class OrdemCompra
{
    public long Id { get; set; }
    public long ContaMasterId { get; set; }

    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public TipoMercado TipoMercado { get; set; }
    public DateTime DataExecucao { get; set; }
}
