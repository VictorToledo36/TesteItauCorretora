using TesteItauCorretora.Domain.Entities;
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
    public bool Ativo { get; private set; } = true;
    public DateTime DataAdesao { get; private set; }
    public DateTime? DataSaida { get; private set; }

    public ICollection<ContaGrafica> ContasGraficas { get; private set; } = new List<ContaGrafica>();
    public ICollection<EventoIR> EventosIR { get; private set; } = new List<EventoIR>();
    public ICollection<Rebalanceamento> Rebalanceamentos { get; private set; } = new List<Rebalanceamento>();

    // Construtor para EF
    private Cliente() { }

    // RN-001, RN-003, RN-005, RN-006
    public Cliente(string nome, string cpf, string email, decimal valorMensal)
    {
        ValidarDadosObrigatorios(nome, cpf, email);
        ValidarValorMensal(valorMensal);

        Nome = nome;
        CPF = LimparCpf(cpf);
        Email = email;
        ValorMensal = valorMensal;
        Ativo = true;
        DataAdesao = DateTime.Now;
    }

    // RN-011, RN-012, RN-013
    public void AlterarValorMensal(decimal novoValor)
    {
        ValidarValorMensal(novoValor);
        // O histórico será mantido através de um evento ou tabela de auditoria
        ValorMensal = novoValor;
    }

    // RN-007, RN-008, RN-009
    public void Sair()
    {
        if (!Ativo)
            throw new ClienteDadosInvalidosException("Cliente já está inativo.");

        Ativo = false;
        DataSaida = DateTime.Now;
        // A posição na custódia é mantida (não há venda automática)
        // O cliente não participa mais das compras programadas
    }

    // RN-010
    public void Reativar()
    {
        if (Ativo)
            throw new ClienteDadosInvalidosException("Cliente já está ativo.");

        Ativo = true;
        DataSaida = null;
    }

    // RN-001
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

    // RN-003
    private void ValidarValorMensal(decimal valor)
    {
        if (valor < VALOR_MINIMO)
            throw new ClienteValorMensalInvalidoException(valor);
    }

    private string LimparCpf(string cpf)
    {
        return cpf.Replace(".", "").Replace("-", "").Trim();
    }

    private bool ValidarFormatoCpf(string cpf)
    {
        var cpfLimpo = LimparCpf(cpf);
        return cpfLimpo.Length == 11 && cpfLimpo.All(char.IsDigit);
    }

    private bool ValidarFormatoEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }
}