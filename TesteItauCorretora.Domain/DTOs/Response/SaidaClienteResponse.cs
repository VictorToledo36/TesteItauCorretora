namespace TesteItauCorretora.Domain.DTOs.Response;
public class SaidaClienteResponse
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataSaida { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}