namespace TesteItauCorretora.Domain.DTOs.Response;

public class CestaAtualResponse
{
    public int CestaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<ItemCestaComCotacaoDto> Itens { get; set; } = new();
}

public class ItemCestaComCotacaoDto
{
    public string Ticker { get; set; } = string.Empty;
    public decimal Percentual { get; set; }
    public decimal CotacaoAtual { get; set; }
}