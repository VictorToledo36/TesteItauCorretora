using TesteItauCorretora.Domain.Exceptions;

namespace TesteItauCorretora.Domain.Entities;

public class CestaRecomendacao
{
    private const int QUANTIDADE_ACOES_OBRIGATORIA = 5;
    private const decimal PERCENTUAL_TOTAL_OBRIGATORIO = 100m;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public bool Ativa { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataDesativacao { get; private set; }
    public ICollection<ItemCesta> Itens { get; private set; } = new List<ItemCesta>();

    // Construtor para EF
    private CestaRecomendacao() { }

    // RN-014, RN-015, RN-016, RN-018
    public CestaRecomendacao(string nome, List<ItemCesta> itens)
    {
        ValidarNome(nome);
        ValidarItens(itens);

        Nome = nome;
        Itens = itens;
        Ativa = true;
        DataCriacao = DateTime.Now;
    }

    // RN-017, RN-018
    public void Desativar()
    {
        if (!Ativa)
            throw new CestaRecomendacaoInvalidaException("Cesta já está desativada.");

        Ativa = false;
        DataDesativacao = DateTime.Now;
    }

    public void Ativar()
    {
        if (Ativa)
            throw new CestaRecomendacaoInvalidaException("Cesta já está ativa.");

        Ativa = true;
        DataDesativacao = null;
    }

    // RN-014, RN-015, RN-016
    private void ValidarItens(List<ItemCesta> itens)
    {
        // RN-014: Deve conter exatamente 5 ações
        if (itens == null || itens.Count != QUANTIDADE_ACOES_OBRIGATORIA)
            throw new CestaRecomendacaoInvalidaException(
                $"A cesta deve conter exatamente {QUANTIDADE_ACOES_OBRIGATORIA} ações.");

        // RN-016: Cada percentual deve ser maior que 0%
        if (itens.Any(i => i.Percentual <= 0))
            throw new CestaRecomendacaoInvalidaException(
                "Todos os percentuais devem ser maiores que 0%.");

        // RN-015: A soma dos percentuais deve ser exatamente 100%
        var somaPercentuais = itens.Sum(i => i.Percentual);
        if (somaPercentuais != PERCENTUAL_TOTAL_OBRIGATORIO)
            throw new CestaRecomendacaoInvalidaException(
                $"A soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentuais}%");

        // Validar se não há ações duplicadas
        var acoesDistintas = itens.Select(i => i.Ticker).Distinct().Count();
        if (acoesDistintas != itens.Count)
            throw new CestaRecomendacaoInvalidaException(
                "Não pode haver ações duplicadas na cesta.");
    }

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new CestaRecomendacaoInvalidaException("Nome da cesta é obrigatório.");
    }

    // Método auxiliar para comparar cestas e identificar mudanças
    public DiferencaCesta CompararCom(CestaRecomendacao cestaAntiga)
    {
        var tickersNovos = Itens.Select(i => i.Ticker).ToHashSet();
        var tickersAntigos = cestaAntiga.Itens.Select(i => i.Ticker).ToHashSet();

        var acoesQueEntraram = tickersNovos.Except(tickersAntigos).ToList();
        var acoesQueSairam = tickersAntigos.Except(tickersNovos).ToList();
        var acoesQueMudaram = new List<MudancaPercentual>();
        var acoesSemAlteracao = new List<string>();

        foreach (var itemNovo in Itens)
        {
            var itemAntigo = cestaAntiga.Itens.FirstOrDefault(i => i.Ticker == itemNovo.Ticker);
            if (itemAntigo != null)
            {
                if (itemAntigo.Percentual != itemNovo.Percentual)
                {
                    acoesQueMudaram.Add(new MudancaPercentual
                    {
                        Ticker = itemNovo.Ticker,
                        PercentualAntigo = itemAntigo.Percentual,
                        PercentualNovo = itemNovo.Percentual
                    });
                }
                else
                {
                    acoesSemAlteracao.Add(itemNovo.Ticker);
                }
            }
        }

        return new DiferencaCesta
        {
            AcoesQueEntraram = acoesQueEntraram,
            AcoesQueSairam = acoesQueSairam,
            AcoesQueMudaram = acoesQueMudaram,
            AcoesSemAlteracao = acoesSemAlteracao
        };
    }
}

// Classes auxiliares para comparação de cestas
public class DiferencaCesta
{
    public List<string> AcoesQueEntraram { get; set; } = new();
    public List<string> AcoesQueSairam { get; set; } = new();
    public List<MudancaPercentual> AcoesQueMudaram { get; set; } = new();
    public List<string> AcoesSemAlteracao { get; set; } = new();
}

public class MudancaPercentual
{
    public string Ticker { get; set; } = string.Empty;
    public decimal PercentualAntigo { get; set; }
    public decimal PercentualNovo { get; set; }
}