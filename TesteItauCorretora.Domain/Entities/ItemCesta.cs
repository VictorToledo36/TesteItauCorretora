using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class ItemCesta
{
    public long Id { get; set; }
    public long CestaId { get; set; }

    public string Ticker { get; set; } = string.Empty;
    public decimal Percentual { get; set; }

    public CestaRecomendacao? Cesta { get; set; }
}
