using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Exceptions;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class ItemCestaTests
{
    [Fact]
    public void Criar_ItemCestaValido_DeveCriarComSucesso()
    {
        var ticker = "PETR4";
        var percentual = 20m;

        var item = new ItemCesta(ticker, percentual);

        item.Ticker.Should().Be("PETR4");
        item.Percentual.Should().Be(percentual);
    }

    [Fact]
    public void Criar_ItemCestaComTickerMinusculo_DeveConverterParaMaiusculo()
    {
        var item = new ItemCesta("petr4", 20m);

        item.Ticker.Should().Be("PETR4");
    }

    [Fact]
    public void Criar_ItemCestaComTickerComEspacos_DeveRemoverEspacos()
    {
        var item = new ItemCesta("PETR4 ", 20m);

        item.Ticker.Should().Be("PETR4");
    }

    [Fact]
    public void Criar_ItemCestaComTickerVazio_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("", 20m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Ticker é obrigatório.");
    }

    [Fact]
    public void Criar_ItemCestaComTickerMuitoCurto_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("PET", 20m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Ticker deve ter entre 4 e 6 caracteres.");
    }

    [Fact]
    public void Criar_ItemCestaComTickerMuitoLongo_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("PETR4567", 20m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Ticker deve ter entre 4 e 6 caracteres.");
    }

    [Fact]
    public void Criar_ItemCestaComTicker4Caracteres_DeveCriarComSucesso()
    {
        var item = new ItemCesta("PETR", 20m);

        item.Ticker.Should().Be("PETR");
    }

    [Fact]
    public void Criar_ItemCestaComTicker6Caracteres_DeveCriarComSucesso()
    {
        var item = new ItemCesta("PETR4F", 20m);

        item.Ticker.Should().Be("PETR4F");
    }

    [Fact]
    public void Criar_ItemCestaComPercentualZero_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("PETR4", 0m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Percentual deve ser maior que 0.");
    }

    [Fact]
    public void Criar_ItemCestaComPercentualNegativo_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("PETR4", -10m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Percentual deve ser maior que 0.");
    }

    [Fact]
    public void Criar_ItemCestaComPercentualMaiorQue100_DeveLancarExcecao()
    {
        var act = () => new ItemCesta("PETR4", 101m);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Percentual não pode ser maior que 100.");
    }

    [Fact]
    public void Criar_ItemCestaComPercentual100_DeveCriarComSucesso()
    {
        var item = new ItemCesta("PETR4", 100m);

        item.Percentual.Should().Be(100m);
    }

    [Fact]
    public void Criar_ItemCestaComPercentualDecimal_DeveCriarComSucesso()
    {
        var item = new ItemCesta("PETR4", 16.67m);

        item.Percentual.Should().Be(16.67m);
    }
}
