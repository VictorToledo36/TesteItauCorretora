using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class ContaGraficaTests
{
    [Fact]
    public void Criar_ContaGraficaComNumeroETipo_DeveCriarComSucesso()
    {
        var numeroConta = "FLH-000001";
        var tipo = TipoConta.Filhote;

        var conta = new ContaGrafica(numeroConta, tipo);

        conta.NumeroConta.Should().Be(numeroConta);
        conta.Tipo.Should().Be(tipo);
        conta.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarParaCliente_DeveGerarContaTipoFilhote()
    {
        var clienteId = 1;

        var conta = ContaGrafica.CriarParaCliente(clienteId);

        conta.ClienteId.Should().Be(clienteId);
        conta.Tipo.Should().Be(TipoConta.Filhote);
        conta.NumeroConta.Should().StartWith("CG-");
        conta.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarParaCliente_DeveGerarNumeroContaUnico()
    {
        var clienteId = 1;

        var conta1 = ContaGrafica.CriarParaCliente(clienteId);
        var conta2 = ContaGrafica.CriarParaCliente(clienteId);

        conta1.NumeroConta.Should().NotBe(conta2.NumeroConta);
    }

    [Fact]
    public void AssociarCliente_DeveAssociarClienteAConta()
    {
        var conta = new ContaGrafica("MST-000001", TipoConta.Master);
        var clienteId = 5;

        conta.AssociarCliente(clienteId);

        conta.ClienteId.Should().Be(clienteId);
    }

    [Fact]
    public void Criar_ContaTipoMaster_DeveCriarComSucesso()
    {
        var conta = new ContaGrafica("MST-000001", TipoConta.Master);

        conta.Tipo.Should().Be(TipoConta.Master);
        conta.NumeroConta.Should().Be("MST-000001");
    }

    [Fact]
    public void Criar_ContaTipoFilhote_DeveCriarComSucesso()
    {
        var conta = new ContaGrafica("FLH-000001", TipoConta.Filhote);

        conta.Tipo.Should().Be(TipoConta.Filhote);
        conta.NumeroConta.Should().Be("FLH-000001");
    }

    [Fact]
    public void CriarParaCliente_MultiplasChamadas_DeveGerarNumerosContaDiferentes()
    {
        var clienteId = 1;
        var contas = new List<ContaGrafica>();

        for (int i = 0; i < 10; i++)
        {
            contas.Add(ContaGrafica.CriarParaCliente(clienteId));
        }

        var numerosUnicos = contas.Select(c => c.NumeroConta).Distinct().Count();
        numerosUnicos.Should().Be(10);
    }

    [Fact]
    public void NumeroConta_GeradoAutomaticamente_DeveTerFormatoCorreto()
    {
        var conta = ContaGrafica.CriarParaCliente(1);

        conta.NumeroConta.Should().MatchRegex(@"^CG-[A-Z0-9]{8}$");
    }

    [Fact]
    public void Custodias_ContaNova_DeveIniciarVazia()
    {
        var conta = new ContaGrafica("FLH-000001", TipoConta.Filhote);

        conta.Custodias.Should().NotBeNull();
        conta.Custodias.Should().BeEmpty();
    }

    [Fact]
    public void TipoConta_Master_DeveSerValor1()
    {
        ((int)TipoConta.Master).Should().Be(1);
    }

    [Fact]
    public void TipoConta_Filhote_DeveSerValor2()
    {
        ((int)TipoConta.Filhote).Should().Be(2);
    }

    [Fact]
    public void Criar_ContaComNumeroVazio_DeveLancarExcecao()
    {
        var act = () => new ContaGrafica("", TipoConta.Filhote);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AssociarCliente_DuasVezes_DeveAtualizarClienteId()
    {
        var conta = new ContaGrafica("MST-000001", TipoConta.Master);

        conta.AssociarCliente(1);
        conta.AssociarCliente(2);

        conta.ClienteId.Should().Be(2);
    }

    [Fact]
    public void ClienteId_ContaRecemCriada_DeveSerNuloOuZero()
    {
        var conta = new ContaGrafica("FLH-000001", TipoConta.Filhote);

        // ClienteId é int, então começa como 0 antes de associar
        conta.ClienteId.Should().Be(0);
    }

    [Fact]
    public void CriarParaCliente_ClientesdiferentesIds_DeveGerarContasDiferentes()
    {
        var conta1 = ContaGrafica.CriarParaCliente(1);
        var conta2 = ContaGrafica.CriarParaCliente(2);

        conta1.ClienteId.Should().Be(1);
        conta2.ClienteId.Should().Be(2);
        conta1.NumeroConta.Should().NotBe(conta2.NumeroConta);
    }
}
