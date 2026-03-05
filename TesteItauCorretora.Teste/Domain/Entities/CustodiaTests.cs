using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class CustodiaTests
{
    [Fact]
    public void AdicionarAtivos_QuantidadeValida_DeveAdicionarECalcularPrecoMedio()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100,
            PrecoMedio = 30m
        };

        custodia.AdicionarAtivos(50, 36m);

        custodia.Quantidade.Should().Be(150);
        custodia.PrecoMedio.Should().Be(32m); // (100*30 + 50*36) / 150 = 32
    }

    [Fact]
    public void AdicionarAtivos_PrimeirosAtivos_DeveDefinirPrecoMedio()
    {
        var custodia = new Custodia
        {
            Ticker = "VALE3",
            Quantidade = 0,
            PrecoMedio = 0
        };

        custodia.AdicionarAtivos(100, 50m);

        custodia.Quantidade.Should().Be(100);
        custodia.PrecoMedio.Should().Be(50m);
    }

    [Fact]
    public void AdicionarAtivos_QuantidadeZero_DeveLancarExcecao()
    {
        var custodia = new Custodia { Ticker = "PETR4" };

        var act = () => custodia.AdicionarAtivos(0, 30m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarAtivos_QuantidadeNegativa_DeveLancarExcecao()
    {
        var custodia = new Custodia { Ticker = "PETR4" };

        var act = () => custodia.AdicionarAtivos(-10, 30m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarAtivos_DeveAtualizarDataUltimaAtualizacao()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100,
            PrecoMedio = 30m,
            DataUltimaAtualizacao = DateTime.Now.AddDays(-1)
        };

        custodia.AdicionarAtivos(50, 36m);

        custodia.DataUltimaAtualizacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoverAtivos_QuantidadeValida_DeveRemoverAtivos()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100,
            PrecoMedio = 30m
        };

        custodia.RemoverAtivos(30);

        custodia.Quantidade.Should().Be(70);
    }

    [Fact]
    public void RemoverAtivos_QuantidadeZero_DeveLancarExcecao()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100
        };

        var act = () => custodia.RemoverAtivos(0);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void RemoverAtivos_QuantidadeNegativa_DeveLancarExcecao()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100
        };

        var act = () => custodia.RemoverAtivos(-10);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void RemoverAtivos_QuantidadeMaiorQueDisponivel_DeveLancarExcecao()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 50
        };

        var act = () => custodia.RemoverAtivos(100);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Quantidade insuficiente em custódia.");
    }

    [Fact]
    public void RemoverAtivos_DeveAtualizarDataUltimaAtualizacao()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100,
            DataUltimaAtualizacao = DateTime.Now.AddDays(-1)
        };

        custodia.RemoverAtivos(30);

        custodia.DataUltimaAtualizacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoverAtivos_RemoverTodosAtivos_DeveZerarQuantidade()
    {
        var custodia = new Custodia
        {
            Ticker = "PETR4",
            Quantidade = 100,
            PrecoMedio = 30m
        };

        custodia.RemoverAtivos(100);

        custodia.Quantidade.Should().Be(0);
    }

    [Fact]
    public void AdicionarAtivos_CalculoPrecoMedioComplexo_DeveCalcularCorretamente()
    {
        var custodia = new Custodia
        {
            Ticker = "ITUB4",
            Quantidade = 200,
            PrecoMedio = 25m
        };

        custodia.AdicionarAtivos(100, 28m);

        // (200 * 25 + 100 * 28) / 300 = (5000 + 2800) / 300 = 26
        custodia.PrecoMedio.Should().Be(26m);
        custodia.Quantidade.Should().Be(300);
    }
}
