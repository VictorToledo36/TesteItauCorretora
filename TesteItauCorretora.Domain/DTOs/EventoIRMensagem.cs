namespace TesteItauCorretora.Domain.DTOs;
public class EventoIRMensagem
{
    public int ClienteId { get; set; }
    public string CPF { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorOperacao { get; set; }
    public decimal ValorIR { get; set; }
    public string TipoEvento { get; set; } = string.Empty; 
    public DateTime DataEvento { get; set; }
}