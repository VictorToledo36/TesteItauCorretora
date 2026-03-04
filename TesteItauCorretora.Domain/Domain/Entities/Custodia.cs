using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class Custodia
{
    public int Id { get; set; }
    public int ContaGraficaId { get; set; }
    public ContaGrafica ContaGrafica { get; private set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
    public ICollection<Distribuicao> Distribuicao { get; set; } = new List<Distribuicao>();

}
