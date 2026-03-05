using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Exceptions;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Criar_ClienteValido_DeveCriarComSucesso()
    {
        var nome = "João da Silva";
        var cpf = "12345678901";
        var email = "joao@email.com";
        var valorMensal = 1000m;

        var cliente = new Cliente(nome, cpf, email, valorMensal);

        cliente.Nome.Should().Be(nome);
        cliente.CPF.Should().Be("12345678901");
        cliente.Email.Should().Be(email);
        cliente.ValorMensal.Should().Be(valorMensal);
        cliente.Ativo.Should().BeTrue();
        cliente.DataAdesao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Criar_ClienteComNomeVazio_DeveLancarExcecao()
    {
        var act = () => new Cliente("", "12345678901", "joao@email.com", 1000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Nome é obrigatório.");
    }

    [Fact]
    public void Criar_ClienteComCpfVazio_DeveLancarExcecao()
    {
        var act = () => new Cliente("João", "", "joao@email.com", 1000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("CPF é obrigatório.");
    }

    [Fact]
    public void Criar_ClienteComEmailVazio_DeveLancarExcecao()
    {
        var act = () => new Cliente("João", "12345678901", "", 1000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Email é obrigatório.");
    }

    [Fact]
    public void Criar_ClienteComCpfInvalido_DeveLancarExcecao()
    {
        var act = () => new Cliente("João", "123", "joao@email.com", 1000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("CPF inválido.");
    }

    [Fact]
    public void Criar_ClienteComEmailInvalido_DeveLancarExcecao()
    {
        var act = () => new Cliente("João", "12345678901", "emailinvalido", 1000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Email inválido.");
    }

    [Fact]
    public void Criar_ClienteComValorMensalAbaixoDoMinimo_DeveLancarExcecao()
    {
        var act = () => new Cliente("João", "12345678901", "joao@email.com", 50m);

        act.Should().Throw<ClienteValorMensalInvalidoException>();
    }

    [Fact]
    public void Criar_ClienteComValorMensalMinimo_DeveCriarComSucesso()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 100m);

        cliente.ValorMensal.Should().Be(100m);
    }

    [Fact]
    public void Criar_ClienteComCpfFormatado_DeveLimparFormatacao()
    {
        var cliente = new Cliente("João", "123.456.789-01", "joao@email.com", 1000m);

        cliente.CPF.Should().Be("12345678901");
    }

    [Fact]
    public void AlterarValorMensal_ClienteAtivo_DeveAlterarECriarHistorico()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        var novoValor = 2000m;

        cliente.AlterarValorMensal(novoValor, "Aumento de salário");

        cliente.ValorMensal.Should().Be(novoValor);
        cliente.HistoricoValores.Should().HaveCount(1);
        cliente.HistoricoValores.First().ValorAnterior.Should().Be(1000m);
        cliente.HistoricoValores.First().ValorNovo.Should().Be(2000m);
    }

    [Fact]
    public void AlterarValorMensal_ClienteInativo_DeveLancarExcecao()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        cliente.Sair();

        var act = () => cliente.AlterarValorMensal(2000m);

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Cliente inativo não pode alterar valor.");
    }

    [Fact]
    public void AlterarValorMensal_MesmoValor_NaoDeveCriarHistorico()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1000m);

        cliente.ValorMensal.Should().Be(1000m);
        cliente.HistoricoValores.Should().BeEmpty();
    }

    [Fact]
    public void Sair_ClienteAtivo_DeveInativarCliente()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.Sair();

        cliente.Ativo.Should().BeFalse();
        cliente.DataSaida.Should().NotBeNull();
        cliente.DataSaida.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Sair_ClienteJaInativo_DeveLancarExcecao()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        cliente.Sair();

        var act = () => cliente.Sair();

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Cliente já está inativo.");
    }

    [Fact]
    public void Reativar_ClienteInativo_DeveReativarCliente()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        cliente.Sair();

        cliente.Reativar();

        cliente.Ativo.Should().BeTrue();
        cliente.DataSaida.Should().BeNull();
    }

    [Fact]
    public void Reativar_ClienteJaAtivo_DeveLancarExcecao()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        var act = () => cliente.Reativar();

        act.Should().Throw<ClienteDadosInvalidosException>()
            .WithMessage("Cliente já está ativo.");
    }
}
