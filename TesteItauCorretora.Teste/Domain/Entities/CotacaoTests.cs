using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class CotacaoTests
{
    [Fact]
    public void Criar_CotacaoValida_DeveCriarComSucesso()
    {
        var cotacao = new Cotacao
        {
            Ticker = "PETR4",
            DataPregao = new DateTime(2026, 3, 5),
            PrecoAbertura = 30m,
            PrecoFechamento = 32m,
            PrecoMaximo = 33m,
            PrecoMinimo = 29m
        };

        cotacao.Ticker.Should().Be("PETR4");
        cotacao.DataPregao.Should().Be(new DateTime(2026, 3, 5));
        cotacao.PrecoAbertura.Should().Be(30m);
        cotacao.PrecoFechamento.Should().Be(32m);
        cotacao.PrecoMaximo.Should().Be(33m);
        cotacao.PrecoMinimo.Should().Be(29m);
    }

    [Fact]
    public void Cotacao_CalcularVariacao_DeveCalcularCorretamente()
    {
        var cotacao = new Cotacao
        {
            PrecoAbertura = 30m,
            PrecoFechamento = 33m
        };

        var variacao = cotacao.PrecoFechamento - cotacao.PrecoAbertura;
        var variacaoPercentual = (variacao / cotacao.PrecoAbertura) * 100;

        variacao.Should().Be(3m);
        variacaoPercentual.Should().Be(10m);
    }

    [Fact]
    public void Cotacao_PrecoMaximoMaiorQueMinimo_DeveSerValido()
    {
        var cotacao = new Cotacao
        {
            PrecoMaximo = 35m,
            PrecoMinimo = 28m
        };

        cotacao.PrecoMaximo.Should().BeGreaterThan(cotacao.PrecoMinimo);
    }

    [Fact]
    public void Cotacao_PrecoFechamentoDentroDoRange_DeveSerValido()
    {
        var cotacao = new Cotacao
        {
            PrecoMinimo = 28m,
            PrecoFechamento = 32m,
            PrecoMaximo = 35m
        };

        cotacao.PrecoFechamento.Should().BeGreaterOrEqualTo(cotacao.PrecoMinimo);
        cotacao.PrecoFechamento.Should().BeLessOrEqualTo(cotacao.PrecoMaximo);
    }

    [Fact]
    public void Cotacao_PrecoAberturaDentroDoRange_DeveSerValido()
    {
        var cotacao = new Cotacao
        {
            PrecoMinimo = 28m,
            PrecoAbertura = 30m,
            PrecoMaximo = 35m
        };

        cotacao.PrecoAbertura.Should().BeGreaterOrEqualTo(cotacao.PrecoMinimo);
        cotacao.PrecoAbertura.Should().BeLessOrEqualTo(cotacao.PrecoMaximo);
    }

    [Fact]
    public void Cotacao_ComPrecosDecimais_DeveArmazenarCorretamente()
    {
        var cotacao = new Cotacao
        {
            PrecoAbertura = 30.55m,
            PrecoFechamento = 32.75m,
            PrecoMaximo = 33.10m,
            PrecoMinimo = 29.80m
        };

        cotacao.PrecoAbertura.Should().Be(30.55m);
        cotacao.PrecoFechamento.Should().Be(32.75m);
        cotacao.PrecoMaximo.Should().Be(33.10m);
        cotacao.PrecoMinimo.Should().Be(29.80m);
    }

    [Fact]
    public void Cotacao_DataPregao_DeveArmazenarCorretamente()
    {
        var dataEsperada = new DateTime(2026, 3, 5);

        var cotacao = new Cotacao
        {
            DataPregao = dataEsperada
        };

        cotacao.DataPregao.Should().Be(dataEsperada);
    }

    [Fact]
    public void Cotacao_TickerVazio_DevePermitirInicializacao()
    {
        var cotacao = new Cotacao();

        cotacao.Ticker.Should().BeEmpty();
    }

    [Fact]
    public void Cotacao_CalcularAmplitude_DeveCalcularCorretamente()
    {
        var cotacao = new Cotacao
        {
            PrecoMaximo = 35m,
            PrecoMinimo = 28m
        };

        var amplitude = cotacao.PrecoMaximo - cotacao.PrecoMinimo;

        amplitude.Should().Be(7m);
    }

    [Fact]
    public void Cotacao_VariacaoNegativa_DeveCalcularCorretamente()
    {
        var cotacao = new Cotacao
        {
            PrecoAbertura = 35m,
            PrecoFechamento = 32m
        };

        var variacao = cotacao.PrecoFechamento - cotacao.PrecoAbertura;

        variacao.Should().Be(-3m);
    }
}
