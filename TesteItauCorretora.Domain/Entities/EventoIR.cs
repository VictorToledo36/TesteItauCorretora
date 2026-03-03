using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public enum TipoEventoIR
{
    DedoDuro = 1,
    Venda = 2
}

public class EventoIR
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public TipoEventoIR Tipo { get; set; }
    public decimal ValorBase { get; set; }
    public decimal ValorIR { get; set; }
    public bool PublicadoKafka { get; set; }
    public DateTime DataEvento { get; set; }
}
