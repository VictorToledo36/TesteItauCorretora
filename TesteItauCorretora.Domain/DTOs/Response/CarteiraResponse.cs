namespace TesteItauCorretora.Domain.DTOs.Response;
public class CarteiraResponse
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string ContaGrafica { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public ResumoCarteiraDto Resumo { get; set; } = new();
    public List<AtivoCarteiraDto> Ativos { get; set; } = new();
}

public class ResumoCarteiraDto
{
    public decimal ValorTotalInvestido { get; set; }
    public decimal ValorAtualCarteira { get; set; }
    public decimal PlTotal { get; set; }
    public decimal RentabilidadePercentual { get; set; }
}

public class AtivoCarteiraDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal CotacaoAtual { get; set; }
    public decimal ValorAtual { get; set; }
    public decimal Pl { get; set; }
    public decimal PlPercentual { get; set; }
    public decimal ComposicaoCarteira { get; set; }
}