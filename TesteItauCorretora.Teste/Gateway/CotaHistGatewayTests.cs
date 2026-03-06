using FluentAssertions;
using TesteItauCorretora.Infrastructure.Gateway;
using Xunit;

namespace TesteItauCorretora.Teste.Gateway;

public class CotaHistGatewayTests
{
    private const string PastaCotacoes = @"E:\Projetos\TesteItauCorretora\Cotacoes";

    [Fact]
    public async Task ObterUltimaCotacao_TickerExistente_DeveRetornarCotacao()
    {
        // Arrange
        var gateway = new CotaHistGateway(PastaCotacoes);

        // Act
        var cotacao = await gateway.ObterUltimaCotacaoAsync("PETR4");

        // Assert
        cotacao.Should().NotBeNull();
        cotacao!.Ticker.Should().Be("PETR4");
        cotacao.PrecoFechamento.Should().BeGreaterThan(0);
        cotacao.DataPregao.Should().NotBe(default);
    }

    [Fact]
    public async Task ObterUltimaCotacao_TickerInexistente_DeveRetornarNull()
    {
        // Arrange
        var gateway = new CotaHistGateway(PastaCotacoes);

        // Act
        var cotacao = await gateway.ObterUltimaCotacaoAsync("TICKER_INEXISTENTE");

        // Assert
        cotacao.Should().BeNull();
    }

    [Fact]
    public async Task ObterUltimasCotacoes_MultiplosTickers_DeveRetornarDicionario()
    {
        // Arrange
        var gateway = new CotaHistGateway(PastaCotacoes);
        var tickers = new[] { "PETR4", "VALE3", "ITUB4" };

        // Act
        var cotacoes = await gateway.ObterUltimasCotacoesAsync(tickers);

        // Assert
        cotacoes.Should().NotBeEmpty();
        cotacoes.Should().ContainKey("PETR4");
        cotacoes.Should().ContainKey("VALE3");
        
        foreach (var cotacao in cotacoes.Values)
        {
            cotacao.PrecoFechamento.Should().BeGreaterThan(0);
            cotacao.DataPregao.Should().NotBe(default);
        }
    }

    [Fact]
    public async Task ObterUltimasCotacoes_TickersCaseInsensitive_DeveRetornar()
    {
        // Arrange
        var gateway = new CotaHistGateway(PastaCotacoes);
        var tickers = new[] { "petr4", "PETR4", "PeTr4" };

        // Act
        var cotacoes = await gateway.ObterUltimasCotacoesAsync(tickers);

        // Assert
        cotacoes.Should().HaveCount(1);
        cotacoes.Should().ContainKey("PETR4");
    }

    [Fact]
    public async Task ObterUltimaCotacao_PETR4_DeveRetornarValoresCorretos()
    {
        // Arrange
        var gateway = new CotaHistGateway(PastaCotacoes);

        // Act
        var cotacao = await gateway.ObterUltimaCotacaoAsync("PETR4");

        // Assert
        cotacao.Should().NotBeNull();
        cotacao!.Ticker.Should().Be("PETR4");
        cotacao.DataPregao.Should().Be(new DateTime(2026, 2, 5));
        cotacao.PrecoFechamento.Should().Be(37.00m);
        cotacao.PrecoAbertura.Should().Be(37.40m);
        cotacao.PrecoMaximo.Should().Be(37.61m);
        cotacao.PrecoMinimo.Should().Be(36.90m);
    }
}
