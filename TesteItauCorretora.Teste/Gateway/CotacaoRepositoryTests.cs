using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Infrastructure.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;
using Xunit;

namespace TesteItauCorretora.Teste.Gateway;

public class CotacaoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CotacaoRepository _repository;
    private const string PastaCotacoes = @"E:\Projetos\TesteItauCorretora\Cotacoes";

    public CotacaoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CotacaoRepository(_context, PastaCotacoes);
    }

    [Fact]
    public async Task ObterUltimaCotacao_PrimeiraBusca_DeveBuscarNoArquivoESalvarNoCache()
    {
        // Act
        var cotacao = await _repository.ObterUltimaCotacaoAsync("PETR4");

        // Assert
        cotacao.Should().NotBeNull();
        cotacao!.Ticker.Should().Be("PETR4");
        cotacao.PrecoFechamento.Should().BeGreaterThan(0);

        // Verifica se foi salvo no banco
        var cotacaoNoBanco = await _context.Cotacoes
            .FirstOrDefaultAsync(c => c.Ticker == "PETR4");
        
        cotacaoNoBanco.Should().NotBeNull();
        cotacaoNoBanco!.PrecoFechamento.Should().Be(cotacao.PrecoFechamento);
    }

    [Fact]
    public async Task ObterUltimaCotacao_SegundaBusca_DeveBuscarNoCache()
    {
        // Arrange - Primeira busca para popular o cache
        await _repository.ObterUltimaCotacaoAsync("VALE3");
        var cotacoesNoBancoAntes = await _context.Cotacoes.CountAsync();

        // Act - Segunda busca
        var cotacao = await _repository.ObterUltimaCotacaoAsync("VALE3");

        // Assert
        cotacao.Should().NotBeNull();
        cotacao!.Ticker.Should().Be("VALE3");

        // Verifica que não adicionou nova entrada no banco
        var cotacoesNoBancoDepois = await _context.Cotacoes.CountAsync();
        cotacoesNoBancoDepois.Should().Be(cotacoesNoBancoAntes);
    }

    [Fact]
    public async Task ObterUltimasCotacoes_MultiplosTickers_DeveSalvarTodosNoCache()
    {
        // Arrange
        var tickers = new[] { "PETR4", "VALE3", "ITUB4" };

        // Act
        var cotacoes = await _repository.ObterUltimasCotacoesAsync(tickers);

        // Assert
        cotacoes.Should().NotBeEmpty();
        cotacoes.Should().HaveCountGreaterOrEqualTo(2);

        // Verifica se foram salvos no banco
        var cotacoesNoBanco = await _context.Cotacoes.ToListAsync();
        cotacoesNoBanco.Should().NotBeEmpty();
        
        foreach (var ticker in tickers)
        {
            if (cotacoes.ContainsKey(ticker))
            {
                cotacoesNoBanco.Should().Contain(c => c.Ticker == ticker);
            }
        }
    }

    [Fact]
    public async Task ImportarCotacoes_TickersDaCesta_DeveImportarParaOBanco()
    {
        // Arrange
        var tickersCesta = new[] { "PETR4", "VALE3", "ITUB4", "BBDC4", "ABEV3" };

        // Act
        await _repository.ImportarCotacoesAsync(tickersCesta);

        // Assert
        var cotacoesNoBanco = await _context.Cotacoes.ToListAsync();
        cotacoesNoBanco.Should().NotBeEmpty();
        
        foreach (var ticker in tickersCesta)
        {
            var cotacao = cotacoesNoBanco.FirstOrDefault(c => c.Ticker == ticker);
            if (cotacao != null)
            {
                cotacao.PrecoFechamento.Should().BeGreaterThan(0);
                cotacao.DataPregao.Should().NotBe(default);
            }
        }
    }

    [Fact]
    public async Task ObterUltimaCotacao_CotacaoAntigaNoCache_DeveBuscarNoArquivo()
    {
        // Arrange - Adiciona cotação antiga no cache
        var cotacaoAntiga = new Cotacao
        {
            Ticker = "PETR4",
            DataPregao = new DateTime(2020, 1, 1),
            PrecoFechamento = 30.00m,
            PrecoAbertura = 29.50m,
            PrecoMaximo = 30.50m,
            PrecoMinimo = 29.00m
        };
        _context.Cotacoes.Add(cotacaoAntiga);
        await _context.SaveChangesAsync();

        // Act
        var cotacao = await _repository.ObterUltimaCotacaoAsync("PETR4");

        // Assert
        cotacao.Should().NotBeNull();
        // A cotação do arquivo deve ser mais recente que a de 2020
        cotacao!.DataPregao.Should().BeAfter(cotacaoAntiga.DataPregao);
        cotacao.PrecoFechamento.Should().NotBe(cotacaoAntiga.PrecoFechamento);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
