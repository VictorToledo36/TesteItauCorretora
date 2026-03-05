using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class HistoricoValorMensalTests
{
    [Fact]
    public void Cliente_AlterarValorMensal_DeveCriarHistorico()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(2000m, "Aumento de salário");

        cliente.HistoricoValores.Should().HaveCount(1);
        var historico = cliente.HistoricoValores.First();
        historico.ValorAnterior.Should().Be(1000m);
        historico.ValorNovo.Should().Be(2000m);
        historico.DataAlteracao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void HistoricoValorMensal_ComObservacao_DeveArmazenarObservacao()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1500m, "Promoção no trabalho");

        var historico = cliente.HistoricoValores.First();
        historico.Observacao.Should().Be("Promoção no trabalho");
    }

    [Fact]
    public void HistoricoValorMensal_SemObservacao_DevePermitirNull()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1500m);

        var historico = cliente.HistoricoValores.First();
        historico.Observacao.Should().BeNull();
    }

    [Fact]
    public void HistoricoValorMensal_MultiplasAlteracoes_DeveCriarMultiplosRegistros()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1500m, "Primeira alteração");
        cliente.AlterarValorMensal(2000m, "Segunda alteração");
        cliente.AlterarValorMensal(2500m, "Terceira alteração");

        cliente.HistoricoValores.Should().HaveCount(3);
    }

    [Fact]
    public void HistoricoValorMensal_SequenciaDeAlteracoes_DeveManterOrdemCorreta()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1500m);
        cliente.AlterarValorMensal(2000m);

        var historicos = cliente.HistoricoValores.ToList();
        historicos[0].ValorAnterior.Should().Be(1000m);
        historicos[0].ValorNovo.Should().Be(1500m);
        historicos[1].ValorAnterior.Should().Be(1500m);
        historicos[1].ValorNovo.Should().Be(2000m);
    }

    [Fact]
    public void HistoricoValorMensal_Reducao_DeveRegistrarCorretamente()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 2000m);

        cliente.AlterarValorMensal(1500m, "Redução temporária");

        var historico = cliente.HistoricoValores.First();
        historico.ValorAnterior.Should().Be(2000m);
        historico.ValorNovo.Should().Be(1500m);
        historico.Observacao.Should().Be("Redução temporária");
    }

    [Fact]
    public void HistoricoValorMensal_DataAlteracao_DeveSerDataAtual()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        var dataAntes = DateTime.Now;

        cliente.AlterarValorMensal(1500m);
        var dataDepois = DateTime.Now;

        var historico = cliente.HistoricoValores.First();
        historico.DataAlteracao.Should().BeOnOrAfter(dataAntes);
        historico.DataAlteracao.Should().BeOnOrBefore(dataDepois);
    }

    [Fact]
    public void HistoricoValorMensal_ValoresDecimais_DeveArmazenarCorretamente()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1234.56m);

        cliente.AlterarValorMensal(2345.67m);

        var historico = cliente.HistoricoValores.First();
        historico.ValorAnterior.Should().Be(1234.56m);
        historico.ValorNovo.Should().Be(2345.67m);
    }

    [Fact]
    public void HistoricoValorMensal_ClienteInativo_NaoDeveCriarHistorico()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);
        cliente.Sair();

        var act = () => cliente.AlterarValorMensal(1500m);

        act.Should().Throw<Exception>();
        cliente.HistoricoValores.Should().BeEmpty();
    }

    [Fact]
    public void HistoricoValorMensal_ValorIgual_NaoDeveCriarHistorico()
    {
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 1000m);

        cliente.AlterarValorMensal(1000m);

        cliente.HistoricoValores.Should().BeEmpty();
    }
}
