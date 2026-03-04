namespace TesteItauCorretora.Domain.Entities;

public enum TipoConta
{
    Master = 1,
    Filhote = 2
}

public class ContaGrafica
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public string NumeroConta { get; private set; }
    public TipoConta Tipo { get; private set; }
    public DateTime DataCriacao { get; private set; }

    public ICollection<Custodia> Custodias { get; private set; }
        = new List<Custodia>();

    private ContaGrafica() { } // EF

    private ContaGrafica(int clienteId, TipoConta tipo)
    {
        ClienteId = clienteId;
        Tipo = tipo;
        DataCriacao = DateTime.Now;
        NumeroConta = GerarNumeroConta();
    }

    public static ContaGrafica CriarParaCliente(int clienteId)
    {
        return new ContaGrafica(clienteId, TipoConta.Filhote);
    }

    private string GerarNumeroConta()
    {
        return $"CG-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}