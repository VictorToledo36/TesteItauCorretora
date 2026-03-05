using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public enum TipoRebalanceamento
{
    MudancaCesta = 1,
    Desvio = 2
}

public class Rebalanceamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public TipoRebalanceamento Tipo { get; set; }
    public string TickerVendido { get; set; } = string.Empty;
    public string TickerComprado { get; set; } = string.Empty;
    public decimal ValorVenda { get; set; }
    public DateTime DataRebalanceamento { get; set; }
}
