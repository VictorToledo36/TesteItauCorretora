namespace TesteItauCorretora.Domain.DTOs.Response;

public class ExecutarCompraResponse
{
    public DateTime DataExecucao { get; set; }
    public int TotalClientes { get; set; }
    public decimal TotalConsolidado { get; set; }
    public List<OrdemCompraDto> OrdensCompra { get; set; } = new();
    public List<DistribuicaoClienteDto> Distribuicoes { get; set; } = new();
    public List<ResiduoDto> ResiduosCustMaster { get; set; } = new();
    public int EventosIRPublicados { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

public class OrdemCompraDto
{
    public string Ticker { get; set; } = string.Empty;
    public int QuantidadeTotal { get; set; }
    public List<DetalheOrdemDto> Detalhes { get; set; } = new();
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}

public class DetalheOrdemDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class DistribuicaoClienteDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorAporte { get; set; }
    public List<AtivoDistribuicaoDto> Ativos { get; set; } = new();
}

public class AtivoDistribuicaoDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class ResiduoDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}