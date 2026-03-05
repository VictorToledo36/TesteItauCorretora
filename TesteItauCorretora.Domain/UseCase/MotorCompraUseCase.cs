using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Domain.UseCase;

public class MotorCompra
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICestaRecomendacaoRepository _cestaRepository;
    private readonly ICotacaoRepository _cotacaoRepository;
    private readonly ICustodiaRepository _custodiaRepository;
    private readonly IOrdemCompraRepository _ordemCompraRepository;
    private readonly IContaGraficaRepository _contaGraficaRepository;
    private readonly IEventoIRPublisher _eventoIRPublisher;

    private const decimal TAXA_IR_DEDO_DURO = 0.00005m; // 0,005%
    private const int LOTE_PADRAO = 100;

    public MotorCompra(
        IClienteRepository clienteRepository,
        ICestaRecomendacaoRepository cestaRepository,
        ICotacaoRepository cotacaoRepository,
        ICustodiaRepository custodiaRepository,
        IOrdemCompraRepository ordemCompraRepository,
        IContaGraficaRepository contaGraficaRepository,
        IEventoIRPublisher eventoIRPublisher)
    {
        _clienteRepository = clienteRepository;
        _cestaRepository = cestaRepository;
        _cotacaoRepository = cotacaoRepository;
        _custodiaRepository = custodiaRepository;
        _ordemCompraRepository = ordemCompraRepository;
        _contaGraficaRepository = contaGraficaRepository;
        _eventoIRPublisher = eventoIRPublisher;
    }

    public async Task<ResultadoExecucao> ExecutarCompraAsync(DateTime dataExecucao)
    {
        // Validar se é dia de execução (5, 15 ou 25)
        if (!EhDiaDeExecucao(dataExecucao))
            throw new InvalidOperationException("Data não é dia de execução (5, 15 ou 25).");

        // Validar se é dia útil (segunda a sexta)
        if (!EhDiaUtil(dataExecucao))
            throw new InvalidOperationException("Data não é dia útil.");

        var resultado = new ResultadoExecucao { DataExecucao = dataExecucao };

        // Passo 1: Agrupamento de pedidos
        var clientes = await _clienteRepository.ObterClientesAtivosAsync();
        var clientesLista = clientes.ToList();

        if (!clientesLista.Any())
        {
            resultado.Mensagem = "Nenhum cliente ativo encontrado.";
            return resultado;
        }

        var valorTotalAportes = clientesLista.Sum(c => c.ValorMensal / 3);
        resultado.ValorTotalAportes = valorTotalAportes;

        // Passo 2: Obter cesta ativa
        var cestaAtiva = await _cestaRepository.ObterCestaAtivaAsync();
        if (cestaAtiva == null)
            throw new InvalidOperationException("Nenhuma cesta ativa encontrada.");

        // Passo 3: Obter cotações
        var tickers = cestaAtiva.Itens.Select(i => i.Ticker).ToList();
        var cotacoes = await _cotacaoRepository.ObterUltimasCotacoesAsync(tickers);

        // Passo 4: Obter conta master
        var contaMaster = await _contaGraficaRepository.ObterContaMasterAsync();
        if (contaMaster == null)
            throw new InvalidOperationException("Conta master não encontrada.");

        // Passo 5: Calcular e executar compras por ativo
        foreach (var item in cestaAtiva.Itens)
        {
            if (!cotacoes.TryGetValue(item.Ticker, out var cotacao))
                continue;

            var valorParaAtivo = valorTotalAportes * item.Percentual / 100;
            var quantidadeDesejada = (int)(valorParaAtivo / cotacao.PrecoFechamento);

            if (quantidadeDesejada == 0)
                continue;

            // Passo 6: Considerar saldo da custódia master
            var custodiaMaster = await _custodiaRepository.ObterCustodiaMasterPorTickerAsync(item.Ticker);
            var saldoDisponivel = custodiaMaster?.Quantidade ?? 0;
            var quantidadeAComprar = Math.Max(0, quantidadeDesejada - saldoDisponivel);

            if (quantidadeAComprar > 0)
            {
                // Passo 7: Executar compra (lote padrão + fracionário)
                await ExecutarCompraAtivoAsync(contaMaster.Id, item.Ticker, quantidadeAComprar,
                    cotacao.PrecoFechamento, dataExecucao, resultado);
            }

            // Passo 8: Distribuir para filhotes
            var quantidadeParaDistribuir = saldoDisponivel + quantidadeAComprar;
            await DistribuirParaFilhotesAsync(clientesLista, item.Ticker, quantidadeParaDistribuir,
                cotacao.PrecoFechamento, dataExecucao, resultado);
        }

        resultado.Sucesso = true;
        resultado.Mensagem = "Execução concluída com sucesso.";
        return resultado;
    }

    private async Task ExecutarCompraAtivoAsync(int contaMasterId, string ticker, int quantidade,
        decimal precoUnitario, DateTime dataExecucao, ResultadoExecucao resultado)
    {
        // Calcular lotes padrão e fracionário
        var quantidadeLote = (quantidade / LOTE_PADRAO) * LOTE_PADRAO;
        var quantidadeFracionario = quantidade % LOTE_PADRAO;

        // Registrar ordem de lote padrão
        if (quantidadeLote > 0)
        {
            var ordemLote = new OrdemCompra
            {
                ContaMasterId = contaMasterId,
                Ticker = ticker,
                Quantidade = quantidadeLote,
                PrecoUnitario = precoUnitario,
                TipoMercado = TipoMercado.Lote,
                DataExecucao = dataExecucao
            };
            await _ordemCompraRepository.AdicionarAsync(ordemLote);
            resultado.OrdensExecutadas.Add(ordemLote);
        }

        // Registrar ordem fracionária
        if (quantidadeFracionario > 0)
        {
            var ordemFracionario = new OrdemCompra
            {
                ContaMasterId = contaMasterId,
                Ticker = ticker,
                Quantidade = quantidadeFracionario,
                PrecoUnitario = precoUnitario,
                TipoMercado = TipoMercado.Fracionario,
                DataExecucao = dataExecucao
            };
            await _ordemCompraRepository.AdicionarAsync(ordemFracionario);
            resultado.OrdensExecutadas.Add(ordemFracionario);
        }

        // Atualizar custódia master
        var custodiaMaster = await _custodiaRepository.ObterCustodiaMasterPorTickerAsync(ticker);
        if (custodiaMaster == null)
        {
            var contaMaster = await _contaGraficaRepository.ObterContaMasterAsync();
            custodiaMaster = new Custodia
            {
                ContaGraficaId = contaMaster!.Id,
                Ticker = ticker,
                Quantidade = 0,
                PrecoMedio = 0,
                DataUltimaAtualizacao = dataExecucao
            };
            await _custodiaRepository.AdicionarAsync(custodiaMaster);
        }

        custodiaMaster.AdicionarAtivos(quantidade, precoUnitario);
        await _custodiaRepository.AtualizarAsync(custodiaMaster);
    }

    private async Task DistribuirParaFilhotesAsync(List<Cliente> clientes, string ticker,
        int quantidadeTotal, decimal precoUnitario, DateTime dataExecucao, ResultadoExecucao resultado)
    {
        var valorTotalAportes = clientes.Sum(c => c.ValorMensal / 3);
        var quantidadeDistribuida = 0;

        foreach (var cliente in clientes)
        {
            var valorAporteCliente = cliente.ValorMensal / 3;
            var proporcao = valorAporteCliente / valorTotalAportes;
            var quantidadeCliente = (int)(quantidadeTotal * proporcao);

            if (quantidadeCliente == 0)
                continue;

            quantidadeDistribuida += quantidadeCliente;

            // Obter conta gráfica filhote do cliente
            var contaFilhote = cliente.ContasGraficas.FirstOrDefault(c => c.Tipo == TipoConta.Filhote);
            if (contaFilhote == null)
                continue;

            // Atualizar custódia filhote
            var custodiaFilhote = await _custodiaRepository.ObterCustodiaFilhotePorContaAsync(
                contaFilhote.Id, ticker);

            if (custodiaFilhote == null)
            {
                custodiaFilhote = new Custodia
                {
                    ContaGraficaId = contaFilhote.Id,
                    Ticker = ticker,
                    Quantidade = 0,
                    PrecoMedio = 0,
                    DataUltimaAtualizacao = dataExecucao
                };
                await _custodiaRepository.AdicionarAsync(custodiaFilhote);
            }

            custodiaFilhote.AdicionarAtivos(quantidadeCliente, precoUnitario);
            await _custodiaRepository.AtualizarAsync(custodiaFilhote);

            // Registrar distribuição
            var distribuicao = new Distribuicao
            {
                CustodiaFilhoteId = custodiaFilhote.Id,
                Ticker = ticker,
                Quantidade = quantidadeCliente,
                PrecoUnitario = precoUnitario,
                DataDistribuicao = dataExecucao
            };
            resultado.Distribuicoes.Add(distribuicao);

            // Calcular e publicar IR dedo-duro
            var valorOperacao = quantidadeCliente * precoUnitario;
            var valorIR = valorOperacao * TAXA_IR_DEDO_DURO;

            var eventoIR = new EventoIR
            {
                ClienteId = cliente.Id,
                Ticker = ticker,
                Quantidade = quantidadeCliente,
                ValorOperacao = valorOperacao,
                ValorIR = valorIR,
                TipoEvento = TipoEventoIR.Compra,
                DataEvento = dataExecucao
            };

            await _eventoIRPublisher.PublicarAsync(eventoIR);
            resultado.EventosIR.Add(eventoIR);
        }

        // Resíduos permanecem na custódia master
        var residuo = quantidadeTotal - quantidadeDistribuida;
        if (residuo > 0)
        {
            resultado.ResiduosPorTicker[ticker] = residuo;
        }
    }

    private bool EhDiaDeExecucao(DateTime data)
    {
        return data.Day == 5 || data.Day == 15 || data.Day == 25;
    }

    private bool EhDiaUtil(DateTime data)
    {
        return data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday;
    }
}

public class ResultadoExecucao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataExecucao { get; set; }
    public decimal ValorTotalAportes { get; set; }
    public List<OrdemCompra> OrdensExecutadas { get; set; } = new();
    public List<Distribuicao> Distribuicoes { get; set; } = new();
    public List<EventoIR> EventosIR { get; set; } = new();
    public Dictionary<string, int> ResiduosPorTicker { get; set; } = new();
}
