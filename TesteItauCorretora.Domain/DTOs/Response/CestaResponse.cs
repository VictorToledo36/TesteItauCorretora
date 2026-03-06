namespace TesteItauCorretora.Domain.DTOs;

public class CestaResponse
{
    public int CestaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<ItemCestaRequest> Itens { get; set; } = new();
    public bool RebalanceamentoDisparado { get; set; }
    public string Mensagem { get; set; } = string.Empty;

    public CestaAnteriorResponse? CestaAnteriorDesativada { get; set; }
    public List<string>? AtivosRemovidos { get; set; }
    public List<string>? AtivosAdicionados { get; set; }
}

public class CestaAnteriorResponse
{
    public int CestaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataDesativacao { get; set; }
}