namespace TesteItauCorretora.Domain.Entities;

public class Cotacao
{
    public int Id { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public DateTime DataPregao { get; set; }
    public decimal PrecoAbertura { get; set; }
    public decimal PrecoFechamento { get; set; }
    public decimal PrecoMaximo { get; set; }
    public decimal PrecoMinimo { get; set; }
}
