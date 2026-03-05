namespace TesteItauCorretora.Core.UseCase.Clientes;

public class AdesaoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
}