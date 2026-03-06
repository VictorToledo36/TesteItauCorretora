namespace TesteItauCorretora.Domain.DTOs.Response;

public class AdesaoResponse
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataAdesao { get; set; }
    public ContaGraficaDto ContaGrafica { get; set; } = null!;
}

public class ContaGraficaDto
{
    public int Id { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}
