using TesteItauCorretora.Domain.Exceptions;

namespace TesteItauCorretora.Domain.Entities;

public class Cliente
{
    private const decimal VALOR_MINIMO = 100m;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public decimal ValorMensal { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataAdesao { get; private set; }
    public DateTime? DataSaida { get; private set; }

    public ICollection<ContaGrafica> ContasGraficas { get; private set; } = new List<ContaGrafica>();
    public ICollection<EventoIR> EventosIR { get; private set; } = new List<EventoIR>();
    public ICollection<Rebalanceamento> Rebalanceamentos { get; private set; } = new List<Rebalanceamento>();

    public ICollection<HistoricoValorMensal> HistoricoValores { get; private set; }
        = new List<HistoricoValorMensal>();

    private Cliente() { } 

    public Cliente(string nome, string cpf, string email, decimal valorMensal)
    {
        ValidarDadosObrigatorios(nome, cpf, email);
        ValidarValorMensal(valorMensal);

        Nome = nome.Trim();
        CPF = LimparCpf(cpf);
        Email = email.Trim();
        ValorMensal = valorMensal;
        Ativo = true;
        DataAdesao = DateTime.Now;
    }

    public void AlterarValorMensal(decimal novoValor, string? observacao = null)
    {
        if (!Ativo)
            throw new ClienteDadosInvalidosException("Cliente inativo não pode alterar valor.");

        ValidarValorMensal(novoValor);

        if (novoValor == ValorMensal)
            return;

        var historico = new HistoricoValorMensal(
            ValorMensal,
            novoValor,
            observacao
        );

        HistoricoValores.Add(historico);

        ValorMensal = novoValor;
    }

    public void Sair()
    {
        if (!Ativo)
            throw new ClienteDadosInvalidosException("Cliente já está inativo.");

        Ativo = false;
        DataSaida = DateTime.Now;
    }

    public void Reativar()
    {
        if (Ativo)
            throw new ClienteDadosInvalidosException("Cliente já está ativo.");

        Ativo = true;
        DataSaida = null;
    }

    private void ValidarDadosObrigatorios(string nome, string cpf, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ClienteDadosInvalidosException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ClienteDadosInvalidosException("CPF é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ClienteDadosInvalidosException("Email é obrigatório.");

        if (!ValidarFormatoCpf(cpf))
            throw new ClienteDadosInvalidosException("CPF inválido.");

        if (!ValidarFormatoEmail(email))
            throw new ClienteDadosInvalidosException("Email inválido.");
    }

    private void ValidarValorMensal(decimal valor)
    {
        if (valor < VALOR_MINIMO)
            throw new ClienteValorMensalInvalidoException(valor);
    }

    private string LimparCpf(string cpf)
        => new string(cpf.Where(char.IsDigit).ToArray());

    private bool ValidarFormatoCpf(string cpf)
    {
        var cpfLimpo = LimparCpf(cpf);

        if (cpfLimpo.Length != 11)
            return false;

        if (cpfLimpo.Distinct().Count() == 1)
            return false;

        return true;
    }

    private bool ValidarFormatoEmail(string email)
        => email.Contains("@") && email.Contains(".");
}