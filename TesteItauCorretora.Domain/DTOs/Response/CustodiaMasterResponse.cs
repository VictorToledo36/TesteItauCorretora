namespace TesteItauCorretora.Domain.DTOs.Response;

public class CustodiaMasterResponse
{
    public ContaMasterDto ContaMaster { get; set; } = new();
    public List<ItemCustodiaMasterDto> Custodia { get; set; } = new();
    public decimal ValorTotalResiduo { get; set; }
}

public class ContaMasterDto
{
    public int Id { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class ItemCustodiaMasterDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal ValorAtual { get; set; }
    public string Origem { get; set; } = string.Empty;
}