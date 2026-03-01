using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class Custodia
{
    public long Id { get; set; }
    public long ContaGraficaId { get; set; }

    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }

    public DateTime DataUltimaAtualizacao { get; set; }

    public ContaGrafica? ContaGrafica { get; set; }
}
