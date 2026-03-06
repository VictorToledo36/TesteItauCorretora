namespace TesteItauCorretora.Domain.DTOs.Response;
public class AlterarValorMensalResponse
{
    public int ClienteId { get; set; }
    public decimal ValorMensalAnterior { get; set; }
    public decimal ValorMensalNovo { get; set; }
    public DateTime DataAlteracao { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}