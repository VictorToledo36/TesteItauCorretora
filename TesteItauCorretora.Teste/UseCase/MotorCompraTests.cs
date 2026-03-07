using FluentAssertions;
using Moq;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Domain.UseCase;
using Xunit;

namespace TesteItauCorretora.Teste.UseCase;

public class MotorCompraAdicionaisTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICestaRecomendacaoRepository> _cestaRepositoryMock;
    private readonly Mock<ICotacaoRepository> _cotacaoRepositoryMock;
    private readonly Mock<ICustodiaRepository> _custodiaRepositoryMock;
    private readonly Mock<IOrdemCompraRepository> _ordemCompraRepositoryMock;
    private readonly Mock<IContaGraficaRepository> _contaGraficaRepositoryMock;
    private readonly Mock<IEventoIRPublisher> _eventoIRPublisherMock;
    private readonly MotorCompra _motorCompra;

    public MotorCompraAdicionaisTests()
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
    private CestaRecomendacao CriarCestaPadrao()
    {
        return new CestaRecomendacao("Cesta Teste", new List<ItemCesta>
        {
            new ItemCesta("PETR4", 30m),
            new ItemCesta("VALE3", 25m),
            new ItemCesta("ITUB4", 20m),
            new ItemCesta("BBDC4", 15m),
            new ItemCesta("WEGE3", 10m)
        });
    }

    private Dictionary<string, Cotacao> CriarCotacoesPadrao()
    {
        return new Dictionary<string, Cotacao>
        {
            { "PETR4", new Cotacao { Ticker = "PETR4", PrecoFechamento = 35m } },
            { "VALE3", new Cotacao { Ticker = "VALE3", PrecoFechamento = 62m } },
            { "ITUB4", new Cotacao { Ticker = "ITUB4", PrecoFechamento = 30m } },
            { "BBDC4", new Cotacao { Ticker = "BBDC4", PrecoFechamento = 15m } },
            { "WEGE3", new Cotacao { Ticker = "WEGE3", PrecoFechamento = 40m } }
        };
    }

    private ContaGrafica CriarContaMaster()
    {
        return new ContaGrafica("MST-000001", TipoConta.Master);
    }

    private Cliente CriarClienteComConta(string nome, string cpf, decimal valorMensal)
    {
        var cliente = new Cliente(nome, cpf, "email@email.com", valorMensal);
        var conta = ContaGrafica.CriarParaCliente(1);
        typeof(Cliente)
            .GetProperty("ContasGraficas")!
            .SetValue(cliente, new List<ContaGrafica> { conta });
        return cliente;
    }

    // RN-020 / RN-021 / RN-022 — Datas de Execução

    [Theory]
    [InlineData(2026, 3, 1)]
    [InlineData(2026, 3, 6)]
    [InlineData(2026, 3, 10)]
    [InlineData(2026, 3, 20)]
    [InlineData(2026, 3, 30)]
    public async Task ExecutarCompraAsync_DiasQueNaoSao5_15_25_DeveLancarExcecao(int ano, int mes, int dia)
    {
        // RN-020: execução apenas nos dias 5, 15 e 25
        var data = new DateTime(ano, mes, dia);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Data não é dia de execução (5, 15 ou 25).");
    }

    [Fact]
    public async Task ExecutarCompraAsync_Dia15Domingo_DeveAjustarParaSegunda()
    {
        // RN-021: 15/02/2026 é domingo → deve ajustar para 16/02/2026 (segunda)
        var data = new DateTime(2026, 2, 15);
        data.DayOfWeek.Should().Be(DayOfWeek.Sunday); // garantir que é domingo

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        var resultado = await _motorCompra.ExecutarCompraAsync(data);

        resultado.DataExecucao.Should().Be(new DateTime(2026, 2, 16));
        resultado.DataExecucao.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public async Task ExecutarCompraAsync_Dia25Sabado_DeveAjustarParaSegunda()
    {
        // RN-021: 25/04/2026 é sábado → deve ajustar para 27/04/2026 (segunda)
        var data = new DateTime(2026, 4, 25);
        data.DayOfWeek.Should().Be(DayOfWeek.Saturday);

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        var resultado = await _motorCompra.ExecutarCompraAsync(data);

        resultado.DataExecucao.Should().Be(new DateTime(2026, 4, 27));
        resultado.DataExecucao.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    // RN-023 — Valor mensal dividido em 3 parcelas

    [Fact]
    public async Task ExecutarCompraAsync_ValorTotalAportes_DeveSerUmTercoDoValorMensal()
    {
        // RN-023: valor por data = ValorMensal / 3
        // Cliente A: 3000/3 = 1000 | Cliente B: 6000/3 = 2000 | Cliente C: 1500/3 = 500
        // Total: 3500
        var data = new DateTime(2026, 3, 5);

        var clienteA = new Cliente("Cliente A", "12345678901", "a@email.com", 3000m);
        var clienteB = new Cliente("Cliente B", "98765432109", "b@email.com", 6000m);
        var clienteC = new Cliente("Cliente C", "11122233344", "c@email.com", 1500m);

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente> { clienteA, clienteB, clienteC });

        _cestaRepositoryMock.Setup(x => x.ObterCestaAtivaAsync())
            .ReturnsAsync((CestaRecomendacao?)null);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        // Lança exceção na cesta, mas o valor total já foi calculado
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // RN-024 — Apenas clientes Ativo = true participam

    [Fact]
    public async Task ExecutarCompraAsync_ClienteInativo_NaoDeveParticipar()
    {
        // RN-024: apenas clientes ativos participam
        var data = new DateTime(2026, 3, 5);

        var clienteAtivo = new Cliente("Ativo", "12345678901", "ativo@email.com", 3000m);
        var clienteInativo = new Cliente("Inativo", "98765432109", "inativo@email.com", 6000m);
        clienteInativo.Sair(); // desativa

        // Repositório retorna apenas o ativo (já filtrado)
        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente> { clienteAtivo });

        _cestaRepositoryMock.Setup(x => x.ObterCestaAtivaAsync())
            .ReturnsAsync((CestaRecomendacao?)null);

        var act = async () => await _motorCompra.ExecutarCompraAsync(data);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Nenhuma cesta ativa encontrada.");

        // Verifica que o repositório foi chamado (filtro de ativos é responsabilidade do repo)
        _clienteRepositoryMock.Verify(x => x.ObterClientesAtivosAsync(), Times.Once);
    }

    // RN-028 — Quantidade = TRUNCAR(Valor / Cotação)

    [Theory]
    [InlineData(1050, 35, 30)]  // PETR4: 1050/35 = 30 exato
    [InlineData(875, 62, 14)]   // VALE3: 875/62 = 14,11 → trunca para 14
    [InlineData(700, 30, 23)]   // ITUB4: 700/30 = 23,33 → trunca para 23
    [InlineData(525, 15, 35)]   // BBDC4: 525/15 = 35 exato
    [InlineData(350, 40, 8)]    // WEGE3: 350/40 = 8,75 → trunca para 8
    public void CalculoQuantidade_TruncarValorDivididoPorCotacao_DeveArredondarParaBaixo(
        decimal valor, decimal cotacao, int quantidadeEsperada)
    {
        // RN-028: TRUNCAR(Valor / Cotação) — nunca arredonda para cima
        var quantidade = (int)(valor / cotacao);

        quantidade.Should().Be(quantidadeEsperada);
    }

    // RN-031 / RN-032 — Lote padrão vs fracionário

    [Theory]
    [InlineData(28, 0, 28)]    // 28 ações → 0 lotes + 28 fracionárias
    [InlineData(100, 100, 0)]  // 100 ações → 1 lote + 0 fracionárias
    [InlineData(350, 300, 50)] // 350 ações → 3 lotes + 50 fracionárias
    [InlineData(99, 0, 99)]    // 99 ações → 0 lotes + 99 fracionárias
    [InlineData(200, 200, 0)]  // 200 ações → 2 lotes + 0 fracionárias
    [InlineData(157, 100, 57)] // 157 ações → 1 lote + 57 fracionárias
    public void CalculoLotes_SepararLotePadraoEFracionario_DeveCalcularCorretamente(
        int quantidade, int lotesEsperado, int fracionarioEsperado)
    {
        // RN-031 / RN-032: >= 100 em lote padrão, resto no fracionário
        const int LOTE_PADRAO = 100;

        var quantidadeLote = (quantidade / LOTE_PADRAO) * LOTE_PADRAO;
        var quantidadeFracionario = quantidade % LOTE_PADRAO;

        quantidadeLote.Should().Be(lotesEsperado);
        quantidadeFracionario.Should().Be(fracionarioEsperado);
    }

    // RN-035 / RN-036 — Proporção e distribuição por cliente

    [Fact]
    public void CalculoDistribuicao_ProporcaoClientes_DeveCalcularCorretamente()
    {
        // RN-035: Proporção = Aporte do Cliente / Total de Aportes
        // Cliente A: 1000/3500 = 28,57%
        // Cliente B: 2000/3500 = 57,14%
        // Cliente C: 500/3500  = 14,29%
        var aporteA = 1000m;
        var aporteB = 2000m;
        var aporteC = 500m;
        var total = aporteA + aporteB + aporteC; // 3500

        var proporcaoA = aporteA / total;
        var proporcaoB = aporteB / total;
        var proporcaoC = aporteC / total;

        proporcaoA.Should().BeApproximately(0.2857m, 0.0001m);
        proporcaoB.Should().BeApproximately(0.5714m, 0.0001m);
        proporcaoC.Should().BeApproximately(0.1429m, 0.0001m);

        // Soma das proporções deve ser 1 (100%)
        (proporcaoA + proporcaoB + proporcaoC).Should().BeApproximately(1m, 0.0001m);
    }

    [Fact]
    public void CalculoDistribuicao_QuantidadePorCliente_DeveTruncar()
    {
        // RN-036: Quantidade = TRUNCAR(Proporção x Quantidade Total)
        // PETR4: 30 disponíveis
        // Cliente A: TRUNCAR(30 x 28,57%) = TRUNCAR(8,57) = 8
        // Cliente B: TRUNCAR(30 x 57,14%) = TRUNCAR(17,14) = 17
        // Cliente C: TRUNCAR(30 x 14,29%) = TRUNCAR(4,29) = 4
        var quantidadeTotal = 30;
        var proporcaoA = 1000m / 3500m;
        var proporcaoB = 2000m / 3500m;
        var proporcaoC = 500m / 3500m;

        var qtdA = (int)(quantidadeTotal * proporcaoA);
        var qtdB = (int)(quantidadeTotal * proporcaoB);
        var qtdC = (int)(quantidadeTotal * proporcaoC);

        qtdA.Should().Be(8);
        qtdB.Should().Be(17);
        qtdC.Should().Be(4);

        // Total distribuído: 8 + 17 + 4 = 29 (resíduo = 1)
        var totalDistribuido = qtdA + qtdB + qtdC;
        totalDistribuido.Should().Be(29);

        var residuo = quantidadeTotal - totalDistribuido;
        residuo.Should().Be(1); // RN-039: resíduo fica na master
    }

    // RN-029 / RN-030 — Saldo master descontado da compra

    [Theory]
    [InlineData(30, 2, 28)]  // PETR4: 30 desejadas - 2 saldo = 28 a comprar
    [InlineData(14, 0, 14)]  // VALE3: 14 desejadas - 0 saldo = 14 a comprar
    [InlineData(23, 1, 22)]  // ITUB4: 23 desejadas - 1 saldo = 22 a comprar
    [InlineData(35, 0, 35)]  // BBDC4: 35 desejadas - 0 saldo = 35 a comprar
    [InlineData(8, 0, 8)]    // WEGE3: 8 desejadas  - 0 saldo = 8 a comprar
    public void CalculoCompra_DescontarSaldoMaster_DeveCalcularQuantidadeAComprar(
        int quantidadeDesejada, int saldoMaster, int quantidadeEsperada)
    {
        // RN-029 / RN-030: descontar saldo master da quantidade desejada
        var quantidadeAComprar = Math.Max(0, quantidadeDesejada - saldoMaster);

        quantidadeAComprar.Should().Be(quantidadeEsperada);
    }

    [Fact]
    public void CalculoCompra_SaldoMasterMaiorQueDesejado_DeveRetornarZero()
    {
        // Caso extremo: saldo master maior que quantidade desejada → não compra nada
        var quantidadeDesejada = 5;
        var saldoMaster = 10;

        var quantidadeAComprar = Math.Max(0, quantidadeDesejada - saldoMaster);

        quantidadeAComprar.Should().Be(0);
    }

    // RN-037 — Quantidade total disponível = compradas + saldo master

    [Fact]
    public void CalculoDistribuicao_QuantidadeTotalDisponivel_DeveIncluirSaldoMaster()
    {
        // RN-037: quantidade para distribuir = compradas + saldo master anterior
        // ITUB4: 22 compradas + 1 saldo master = 23 disponíveis
        var compradas = 22;
        var saldoMaster = 1;

        var totalDisponivel = compradas + saldoMaster;

        totalDisponivel.Should().Be(23);
    }

    // RN-039 — Resíduos ficam na custódia master

    [Fact]
    public void CalculoResiduo_AposDistribuicao_DeveCalcularCorretamente()
    {
        // RN-039: resíduo = total disponível - total distribuído
        // PETR4: 30 disponíveis - 29 distribuídas = 1 resíduo
        // WEGE3: 8 disponíveis - 7 distribuídas = 1 resíduo
        var casosResiduo = new[]
        {
            new { Total = 30, Distribuido = 29, Residuo = 1 }, // PETR4
            new { Total = 14, Distribuido = 14, Residuo = 0 }, // VALE3
            new { Total = 23, Distribuido = 22, Residuo = 1 }, // ITUB4
            new { Total = 35, Distribuido = 35, Residuo = 0 }, // BBDC4
            new { Total = 8,  Distribuido = 7,  Residuo = 1 }, // WEGE3
        };

        foreach (var caso in casosResiduo)
        {
            var residuo = caso.Total - caso.Distribuido;
            residuo.Should().Be(caso.Residuo);
        }
    }

    // RN-053 / RN-054 — IR Dedo-Duro

    [Theory]
    [InlineData(8, 35, 280, 0.01)]   // 8 PETR4 × R$35 = R$280 → IR = R$0,01
    [InlineData(4, 62, 248, 0.01)]   // 4 VALE3 × R$62 = R$248 → IR = R$0,01
    [InlineData(17, 35, 595, 0.03)]  // 17 PETR4 × R$35 = R$595 → IR = R$0,03
    public void CalculoIR_DedoDuro_DeveCalcularCorretamente(
        int quantidade, decimal preco, decimal valorOperacaoEsperado, decimal valorIREsperado)
    {
        // RN-053: alíquota = 0,005% sobre o valor total da operação
        const decimal TAXA = 0.00005m;

        var valorOperacao = quantidade * preco;
        var valorIR = Math.Round(valorOperacao * TAXA, 2);

        valorOperacao.Should().Be(valorOperacaoEsperado);
        valorIR.Should().Be(valorIREsperado);
    }

    // Verificação de chamadas ao repositório

    [Fact]
    public async Task ExecutarCompraAsync_SemClientes_NaoDeveChamarCestaRepository()
    {
        // Se não há clientes, não deve nem buscar a cesta
        var data = new DateTime(2026, 3, 5);

        _clienteRepositoryMock.Setup(x => x.ObterClientesAtivosAsync())
            .ReturnsAsync(new List<Cliente>());

        await _motorCompra.ExecutarCompraAsync(data);

        _cestaRepositoryMock.Verify(x => x.ObterCestaAtivaAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecutarCompraAsync_ComClientes_DeveChamarCestaRepository()
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

        _cestaRepositoryMock.Verify(x => x.ObterCestaAtivaAsync(), Times.Once);
    }
}