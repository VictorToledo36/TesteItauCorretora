using FluentAssertions;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Exceptions;
using Xunit;

namespace TesteItauCorretora.Teste.Domain.Entities;

public class CestaRecomendacaoTests
{
    private List<ItemCesta> CriarItensValidos()
    {
        return new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        };
    }

    [Fact]
    public void Criar_CestaValida_DeveCriarComSucesso()
    {
        var nome = "Cesta Conservadora";
        var itens = CriarItensValidos();

        var cesta = new CestaRecomendacao(nome, itens);

        cesta.Nome.Should().Be(nome);
        cesta.Itens.Should().HaveCount(5);
        cesta.Ativa.Should().BeTrue();
        cesta.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        cesta.DataDesativacao.Should().BeNull();
    }

    [Fact]
    public void Criar_CestaComNomeVazio_DeveLancarExcecao()
    {
        var itens = CriarItensValidos();

        var act = () => new CestaRecomendacao("", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Nome da cesta é obrigatório.");
    }

    [Fact]
    public void Criar_CestaComMenosDe5Acoes_DeveLancarExcecao()
    {

        var itens = new List<ItemCesta>
        {
            new ItemCesta("PETR4", 50m),
            new ItemCesta("VALE3", 50m)
        };

        var act = () => new CestaRecomendacao("Cesta Teste", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("A cesta deve conter exatamente 5 ações.");
    }

    [Fact]
    public void Criar_CestaComMaisDe5Acoes_DeveLancarExcecao()
    {
        var itens = new List<ItemCesta>
        {
            new ItemCesta("PETR4", 16.66m),
            new ItemCesta("VALE3", 16.67m),
            new ItemCesta("ITUB4", 16.67m),
            new ItemCesta("BBDC4", 16.67m),
            new ItemCesta("ABEV3", 16.67m),
            new ItemCesta("WEGE3", 16.66m)
        };

        var act = () => new CestaRecomendacao("Cesta Teste", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("A cesta deve conter exatamente 5 ações.");
    }

    [Fact]
    public void Criar_CestaComSomaPercentualDiferenteDe100_DeveLancarExcecao()
    {
        var itens = new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 15m) // Total = 95%
        };

        var act = () => new CestaRecomendacao("Cesta Teste", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("A soma dos percentuais deve ser exatamente 100%. Soma atual: 95%");
    }

    [Fact]
    public void Criar_CestaComPercentualZero_DeveLancarExcecao()
    {
        var act = () => new List<ItemCesta>
        {
            new ItemCesta("PETR4", 25m),
            new ItemCesta("VALE3", 25m),
            new ItemCesta("ITUB4", 25m),
            new ItemCesta("BBDC4", 25m),
            new ItemCesta("ABEV3", 0m)
        };

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Percentual deve ser maior que 0.");
    }

    [Fact]
    public void Criar_CestaComAcoesDuplicadas_DeveLancarExcecao()
    {
        var itens = new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("PETR4", 20m), 
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        };

        var act = () => new CestaRecomendacao("Cesta Teste", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Não pode haver ações duplicadas na cesta.");
    }

    [Fact]
    public void Desativar_CestaAtiva_DeveDesativarComSucesso()
    {
        var cesta = new CestaRecomendacao("Cesta Teste", CriarItensValidos());

        cesta.Desativar();

        cesta.Ativa.Should().BeFalse();
        cesta.DataDesativacao.Should().NotBeNull();
        cesta.DataDesativacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_CestaJaDesativada_DeveLancarExcecao()
    {
        var cesta = new CestaRecomendacao("Cesta Teste", CriarItensValidos());
        cesta.Desativar();

        var act = () => cesta.Desativar();

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Cesta já está desativada.");
    }

    [Fact]
    public void Ativar_CestaDesativada_DeveAtivarComSucesso()
    {
        var cesta = new CestaRecomendacao("Cesta Teste", CriarItensValidos());
        cesta.Desativar();

        cesta.Ativar();

        cesta.Ativa.Should().BeTrue();
        cesta.DataDesativacao.Should().BeNull();
    }

    [Fact]
    public void Ativar_CestaJaAtiva_DeveLancarExcecao()
    {
        var cesta = new CestaRecomendacao("Cesta Teste", CriarItensValidos());

        var act = () => cesta.Ativar();

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Cesta já está ativa.");
    }

    [Fact]
    public void CompararCom_CestaComAcoesNovas_DeveIdentificarAcoesQueEntraram()
    {
        var cestaAntiga = new CestaRecomendacao("Cesta Antiga", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        });

        var cestaNova = new CestaRecomendacao("Cesta Nova", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("WEGE3", 20m) 
        });

        var diferenca = cestaNova.CompararCom(cestaAntiga);

        diferenca.AcoesQueEntraram.Should().Contain("WEGE3");
        diferenca.AcoesQueSairam.Should().Contain("ABEV3");
    }

    [Fact]
    public void CompararCom_CestaComPercentuaisDiferentes_DeveIdentificarMudancas()
    {
        var cestaAntiga = new CestaRecomendacao("Cesta Antiga", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        });

        var cestaNova = new CestaRecomendacao("Cesta Nova", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 30m), 
            new ItemCesta("VALE3", 10m), 
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        });

        var diferenca = cestaNova.CompararCom(cestaAntiga);

        diferenca.AcoesQueMudaram.Should().HaveCount(2);
        diferenca.AcoesQueMudaram.Should().Contain(m => m.Ticker == "PETR4" && m.PercentualNovo == 30m);
        diferenca.AcoesQueMudaram.Should().Contain(m => m.Ticker == "VALE3" && m.PercentualNovo == 10m);
    }

    [Fact]
    public void CompararCom_CestasSemAlteracao_DeveIdentificarAcoesSemMudanca()
    {
        var cestaAntiga = new CestaRecomendacao("Cesta Antiga", CriarItensValidos());
        var cestaNova = new CestaRecomendacao("Cesta Nova", CriarItensValidos());

        var diferenca = cestaNova.CompararCom(cestaAntiga);

        diferenca.AcoesSemAlteracao.Should().HaveCount(5);
        diferenca.AcoesQueEntraram.Should().BeEmpty();
        diferenca.AcoesQueSairam.Should().BeEmpty();
        diferenca.AcoesQueMudaram.Should().BeEmpty();
    }

    [Fact]
    public void Criar_CestaComPercentualNegativo_DeveLancarExcecao()
    {
        var act = () => new List<ItemCesta>
    {
        new ItemCesta("PETR4", -10m),
        new ItemCesta("VALE3", 30m),
        new ItemCesta("ITUB4", 30m),
        new ItemCesta("BBDC4", 30m),
        new ItemCesta("ABEV3", 20m)
    };

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Percentual deve ser maior que 0.");
    }

    [Fact]
    public void Criar_CestaComTickerVazio_DeveLancarExcecao()
    {
        var act = () => new List<ItemCesta>
    {
        new ItemCesta("", 20m),
        new ItemCesta("VALE3", 20m),
        new ItemCesta("ITUB4", 20m),
        new ItemCesta("BBDC4", 20m),
        new ItemCesta("ABEV3", 20m)
    };

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Ticker é obrigatório.");
    }

    [Fact]
    public void Criar_CestaComNomeApenasEspacos_DeveLancarExcecao()
    {
        var itens = CriarItensValidos();

        var act = () => new CestaRecomendacao("   ", itens);

        act.Should().Throw<CestaRecomendacaoInvalidaException>()
            .WithMessage("Nome da cesta é obrigatório.");
    }

    [Fact]
    public void CompararCom_CestasIdenticas_NaoDeveHaverDiferencas()
    {
        var itens = CriarItensValidos();
        var cestaAntiga = new CestaRecomendacao("Cesta A", itens);
        var cestaNova = new CestaRecomendacao("Cesta B", CriarItensValidos());

        var diferenca = cestaNova.CompararCom(cestaAntiga);

        diferenca.AcoesQueEntraram.Should().BeEmpty();
        diferenca.AcoesQueSairam.Should().BeEmpty();
        diferenca.AcoesQueMudaram.Should().BeEmpty();
        diferenca.AcoesSemAlteracao.Should().HaveCount(5);
    }
}
