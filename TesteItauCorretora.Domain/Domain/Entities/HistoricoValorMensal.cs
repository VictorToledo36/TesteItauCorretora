namespace TesteItauCorretora.Domain.Entities;

public class HistoricoValorMensal
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public decimal ValorAnterior { get; private set; }
    public decimal ValorNovo { get; private set; }
    public DateTime DataAlteracao { get; private set; }
    public string? Observacao { get; private set; }

    public Cliente Cliente { get; private set; } = null!;

    // Construtor para EF
    private HistoricoValorMensal() { }

    // RN-013: Manter histórico de alterações
    public HistoricoValorMensal(
        int clienteId,
        decimal valorAnterior,
        decimal valorNovo,
        string? observacao = null)
    {
        ClienteId = clienteId;
        ValorAnterior = valorAnterior;
        ValorNovo = valorNovo;
        DataAlteracao = DateTime.Now;
        Observacao = observacao;
    }
}
