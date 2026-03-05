using FluentAssertions;
using TesteItauCorretora.Domain.Exceptions;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Exceptions;

public class ExceptionsTests
{
    [Fact]
    public void ClienteDadosInvalidosException_ComMensagem_DeveCriarComSucesso()
    {
        var mensagem = "Dados inválidos";

        var exception = new ClienteDadosInvalidosException(mensagem);

        exception.Message.Should().Be(mensagem);
    }

    [Fact]
    public void ClienteValorMensalInvalidoException_ComValor_DeveCriarMensagemCorreta()
    {
        var valor = 50m;

        var exception = new ClienteValorMensalInvalidoException(valor);

        exception.Message.Should().Contain("50");
        exception.Message.Should().Contain("100");
    }

    [Fact]
    public void ClienteCpfDuplicadoException_ComCpf_DeveCriarMensagemCorreta()
    {
        var cpf = "12345678901";

        var exception = new ClienteCpfDuplicadoException(cpf);

        exception.Message.Should().Contain(cpf);
    }

    [Fact]
    public void CestaRecomendacaoInvalidaException_ComMensagem_DeveCriarComSucesso()
    {
        var mensagem = "Cesta inválida";

        var exception = new CestaRecomendacaoInvalidaException(mensagem);

        exception.Message.Should().Be(mensagem);
    }

    [Fact]
    public void CestaAtivaJaExisteException_SemParametros_DeveCriarComMensagemPadrao()
    {
        var exception = new CestaAtivaJaExisteException();

        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ClienteDadosInvalidosException_DeveSerDoTipoException()
    {
        var exception = new ClienteDadosInvalidosException("Teste");

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ClienteValorMensalInvalidoException_DeveSerDoTipoException()
    {
        var exception = new ClienteValorMensalInvalidoException(50m);

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ClienteCpfDuplicadoException_DeveSerDoTipoException()
    {
        var exception = new ClienteCpfDuplicadoException("12345678901");

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void CestaRecomendacaoInvalidaException_DeveSerDoTipoException()
    {
        var exception = new CestaRecomendacaoInvalidaException("Teste");

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void CestaAtivaJaExisteException_DeveSerDoTipoException()
    {
        var exception = new CestaAtivaJaExisteException();

        exception.Should().BeAssignableTo<Exception>();
    }
}
