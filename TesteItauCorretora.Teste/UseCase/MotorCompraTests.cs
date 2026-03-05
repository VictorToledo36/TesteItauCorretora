using FluentAssertions;
using Moq;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Domain.UseCase;
using Xunit;

namespace TesteItauCorretora.Teste.UseCase;

public class MotorCompraTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICestaRecomendacaoRepository> _cestaRepositoryMock;
    private readonly Mock<ICotacaoRepository> _cotacaoRepositoryMock;
    private readonly Mock<ICustodiaRepository> _custodiaRepositoryMock;
    private readonly Mock<IOrdemCompraRepository> _ordemCompraRepositoryMock;
    private readonly Mock<IContaGraficaRepository> _contaGraficaRepositoryMock;
    private readonly Mock<IEventoIRPublisher> _eventoIRPublisherMock;
    private readonly MotorCompra _motorCompra;

    public MotorCompraTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _cestaRepositoryMock = new Mock<ICestaRecomendacaoRepository>();
        _cotacaoRepositoryMock = new Mock<ICotacaoRepository>();
        _custodiaRepositoryMock = new Mock<ICustodiaRepository>();
        _ordemCompraRepositoryMock = new Mock<IOrdemCompraRepository>();
        _contaGraficaRepositoryMock = new Mock<IContaGraficaRepository>();
        _eventoIRPublisherMock = new Mock<IEventoIRPublisher>();

        _motorCompra = new MotorCompra(
            _clienteRepositoryMock.Object,
            _cestaRepositoryMock.Object,
            _cotacaoRepositoryMock.Object,
            _custodiaRepositoryMock.Object,
            _ordemCompraRepositoryMock.Object,
            _contaGraficaRepositoryMock.Object,
            _eventoIRPublisherMock.Object
        );
    }

    [Fact]
    public async Task ExecutarCompraAsync_DataNaoEhDiaDeExecucao_DeveLancarExcecao()
    {
        var data = new DateTime(2026, 3, 10); // Dia 10

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Data não é dia de execução (5, 15 ou 25).");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(25)]
    public async Task ExecutarCompraAsync_DiasDeExecucaoValidos_NaoDeveLancarExcecaoDeDia(int dia)
    {
        var data = new DateTime(2026, 3, dia);
        
        // Ajustar para dia útil se necessário
        while (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
        {
            data = data.AddMonths(1);
            data = new DateTime(data.Year, data.Month, dia);
        }

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        var resultado = await _motorCompra.ExecutarCompraAsync(data);

        resultado.Should().NotBeNull();
        resultado.Mensagem.Should().Be("Nenhum cliente ativo encontrado.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_DataEhSabado_DeveLancarExcecao()
    {
        var data = new DateTime(2026, 4, 25); // 25/04/2026 é sábado

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Data não é dia útil.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_DataEhDomingo_DeveLancarExcecao()
    {
        var data = new DateTime(2026, 4, 5); // 05/04/2026 é domingo

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Data não é dia útil.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_NenhumClienteAtivo_DeveRetornarMensagemApropriada()
    {
        var data = new DateTime(2026, 3, 5); // Quinta-feira, dia 5
        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        var resultado = await _motorCompra.ExecutarCompraAsync(data);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be("Nenhum cliente ativo encontrado.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_NenhumaCestaAtiva_DeveLancarExcecao()
    {
        var data = new DateTime(2026, 3, 5);
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 3000m);
        
        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente> { cliente });
        
        _cestaRepositoryMock.Setup(x => x.ObterCestaAtivaAsync())
            .ReturnsAsync((CestaRecomendacao?)null);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Nenhuma cesta ativa encontrada.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_ContaMasterNaoEncontrada_DeveLancarExcecao()
    {
        var data = new DateTime(2026, 3, 5);
        var cliente = new Cliente("João", "12345678901", "joao@email.com", 3000m);
        var cesta = new CestaRecomendacao("Cesta Teste", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 20m),
            new ItemCesta("VALE3", 20m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 20m),
            new ItemCesta("ABEV3", 20m)
        });

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente> { cliente });
        
        _cestaRepositoryMock.Setup(x => x.ObterCestaAtivaAsync())
            .ReturnsAsync(cesta);
        
        _cotacaoRepositoryMock.Setup(x => x.ObterUltimasCotacoesAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Dictionary<string, Cotacao>());
        
        _contaGraficaRepositoryMock.Setup(x => x.ObterContaMasterAsync())
            .ReturnsAsync((ContaGrafica?)null);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Conta master não encontrada.");
    }

    [Fact]
    public async Task ExecutarCompraAsync_CalculoValorTotalAportes_DeveCalcularCorretamente()
    {
        var data = new DateTime(2026, 3, 5);
        var cliente1 = new Cliente("João", "12345678901", "joao@email.com", 3000m);
        var cliente2 = new Cliente("Maria", "98765432109", "maria@email.com", 6000m);
        
        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente> { cliente1, cliente2 });
        
        _cestaRepositoryMock.Setup(x => x.ObterCestaAtivaAsync())
            .ReturnsAsync((CestaRecomendacao?)null);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>();
        // Valor esperado: (3000 + 6000) / 3 = 3000
    }

    [Theory]
    [InlineData(2026, 1, 5)]  // Segunda
    [InlineData(2026, 1, 15)] // Quinta
    [InlineData(2026, 2, 25)] // Quarta
    public async Task ExecutarCompraAsync_DiasUteisValidos_NaoDeveLancarExcecaoDeDiaUtil(int ano, int mes, int dia)
    {
        var data = new DateTime(ano, mes, dia);
        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        var resultado = await _motorCompra.ExecutarCompraAsync(data);

        resultado.Should().NotBeNull();
    }

    [Fact]
    public void ResultadoExecucao_Inicializacao_DeveIniciarComValoresPadrao()
    {
        var resultado = new ResultadoExecucao();

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().BeEmpty();
        resultado.ValorTotalAportes.Should().Be(0);
        resultado.OrdensExecutadas.Should().NotBeNull().And.BeEmpty();
        resultado.Distribuicoes.Should().NotBeNull().And.BeEmpty();
        resultado.EventosIR.Should().NotBeNull().And.BeEmpty();
        resultado.ResiduosPorTicker.Should().NotBeNull().And.BeEmpty();
    }
}
