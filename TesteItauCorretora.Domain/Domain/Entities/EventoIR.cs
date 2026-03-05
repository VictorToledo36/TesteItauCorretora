using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public enum TipoEventoIR
{
    Compra = 1,
    Venda = 2
}

public class EventoIR
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorOperacao { get; set; }
    public decimal ValorIR { get; set; }
    public TipoEventoIR TipoEvento { get; set; }
    public bool PublicadoKafka { get; set; }
    public DateTime DataEvento { get; set; }
}
