namespace TesteItauCorretora.Domain.DTOs.Response;

public class HistoricoCestasResponse
{
    public List<CestaHistoricoDto> Cestas { get; set; } = new();
}

public class CestaHistoricoDto
{
    public int CestaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataDesativacao { get; set; }
    public List<ItemCestaDto> Itens { get; set; } = new();
}

public class ItemCestaDto
{
    public string Ticker { get; set; } = string.Empty;
    public decimal Percentual { get; set; }
}