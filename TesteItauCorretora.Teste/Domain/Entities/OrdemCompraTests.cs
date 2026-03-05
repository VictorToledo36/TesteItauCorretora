using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class OrdemCompraTests
{
    [Fact]
    public void Criar_OrdemCompraLote_DeveCriarComSucesso()
    {
        var ordem = new OrdemCompra
        {
            ContaMasterId = 1,
            Ticker = "PETR4",
            Quantidade = 100,
            PrecoUnitario = 30m,
            TipoMercado = TipoMercado.Lote,
            DataExecucao = DateTime.Now
        };

        ordem.ContaMasterId.Should().Be(1);
        ordem.Ticker.Should().Be("PETR4");
        ordem.Quantidade.Should().Be(100);
        ordem.PrecoUnitario.Should().Be(30m);
        ordem.TipoMercado.Should().Be(TipoMercado.Lote);
    }

    [Fact]
    public void Criar_OrdemCompraFracionario_DeveCriarComSucesso()
    {
        var ordem = new OrdemCompra
        {
            ContaMasterId = 1,
            Ticker = "VALE3",
            Quantidade = 50,
            PrecoUnitario = 60m,
            TipoMercado = TipoMercado.Fracionario,
            DataExecucao = DateTime.Now
        };

        ordem.TipoMercado.Should().Be(TipoMercado.Fracionario);
        ordem.Quantidade.Should().Be(50);
    }

    [Fact]
    public void TipoMercado_Lote_DeveSerValor1()
    {
        ((int)TipoMercado.Lote).Should().Be(1);
    }

    [Fact]
    public void TipoMercado_Fracionario_DeveSerValor2()
    {
        ((int)TipoMercado.Fracionario).Should().Be(2);
    }

    [Fact]
    public void OrdemCompra_CalcularValorTotal_DeveCalcularCorretamente()
    {
        var ordem = new OrdemCompra
        {
            Quantidade = 100,
            PrecoUnitario = 30m
        };

        var valorTotal = ordem.Quantidade * ordem.PrecoUnitario;

        valorTotal.Should().Be(3000m);
    }

    [Fact]
    public void OrdemCompra_Distribuicoes_DeveIniciarVazia()
    {
        var ordem = new OrdemCompra();

        ordem.Distribuicoes.Should().NotBeNull();
        ordem.Distribuicoes.Should().BeEmpty();
    }

    [Fact]
    public void OrdemCompra_ComQuantidadeGrande_DeveArmazenarCorretamente()
    {
        var ordem = new OrdemCompra
        {
            Quantidade = 10000,
            PrecoUnitario = 100m
        };

        ordem.Quantidade.Should().Be(10000);
        var valorTotal = ordem.Quantidade * ordem.PrecoUnitario;
        valorTotal.Should().Be(1000000m);
    }

    [Fact]
    public void OrdemCompra_ComPrecoDecimal_DeveArmazenarCorretamente()
    {
        var ordem = new OrdemCompra
        {
            Quantidade = 100,
            PrecoUnitario = 30.55m
        };

        ordem.PrecoUnitario.Should().Be(30.55m);
        var valorTotal = ordem.Quantidade * ordem.PrecoUnitario;
        valorTotal.Should().Be(3055m);
    }

    [Fact]
    public void OrdemCompra_DataExecucao_DeveArmazenarCorretamente()
    {
        var dataEsperada = new DateTime(2026, 3, 5);

        var ordem = new OrdemCompra
        {
            DataExecucao = dataEsperada
        };

        ordem.DataExecucao.Should().Be(dataEsperada);
    }

    [Fact]
    public void OrdemCompra_TickerVazio_DevePermitirInicializacao()
    {
        var ordem = new OrdemCompra();

        ordem.Ticker.Should().BeEmpty();
    }
}
