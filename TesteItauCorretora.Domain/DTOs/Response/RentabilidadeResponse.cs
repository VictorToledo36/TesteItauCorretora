// TesteItauCorretora.Domain/DTOs/Response/RentabilidadeResponse.cs
namespace TesteItauCorretora.Domain.DTOs.Response;
public class RentabilidadeResponse
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public RentabilidadeResumoDto Rentabilidade { get; set; } = new();
    public List<HistoricoAporteDto> HistoricoAportes { get; set; } = new();
    public List<EvolucaoCarteiraDto> EvolucaoCarteira { get; set; } = new();
}

public class RentabilidadeResumoDto
{
    public decimal ValorTotalInvestido { get; set; }
    public decimal ValorAtualCarteira { get; set; }
    public decimal PlTotal { get; set; }
    public decimal RentabilidadePercentual { get; set; }
}

public class HistoricoAporteDto
{
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Parcela { get; set; } = string.Empty;
}

public class EvolucaoCarteiraDto
{
    public DateTime Data { get; set; }
    public decimal ValorCarteira { get; set; }
    public decimal ValorInvestido { get; set; }
    public decimal Rentabilidade { get; set; }
}