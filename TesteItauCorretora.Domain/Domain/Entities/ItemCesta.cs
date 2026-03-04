using TesteItauCorretora.Domain.Exceptions;

namespace TesteItauCorretora.Domain.Entities;

public class ItemCesta
{
    public int Id { get; private set; }
    public int CestaRecomendacaoId { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public decimal Percentual { get; private set; }

    public CestaRecomendacao CestaRecomendacao { get; private set; } = null!;

    // Construtor para EF
    private ItemCesta() { }

    public ItemCesta(string ticker, decimal percentual)
    {
        ValidarTicker(ticker);
        ValidarPercentual(percentual);

        Ticker = ticker.ToUpper().Trim();
        Percentual = percentual;
    }

    private void ValidarTicker(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new CestaRecomendacaoInvalidaException("Ticker é obrigatório.");

        if (ticker.Length < 4 || ticker.Length > 6)
            throw new CestaRecomendacaoInvalidaException("Ticker deve ter entre 4 e 6 caracteres.");
    }

    private void ValidarPercentual(decimal percentual)
    {
        if (percentual <= 0)
            throw new CestaRecomendacaoInvalidaException("Percentual deve ser maior que 0.");

        if (percentual > 100)
            throw new CestaRecomendacaoInvalidaException("Percentual não pode ser maior que 100.");
    }
}