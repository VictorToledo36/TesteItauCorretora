using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TesteItauCorretora.Domain.DTOs.Request;
using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.UseCase;

namespace TesteItauCorretora.Controllers;

[ApiController]
[Route("api/motor")]
[Produces("application/json")]
[SwaggerTag("Motor de Compra — Execução automatizada de compras programadas nos dias 5, 15 e 25.")]
public class MotorCompraController : ControllerBase
{
    private readonly MotorCompra _motorCompra;

    public MotorCompraController(MotorCompra motorCompra)
    {
        _motorCompra = motorCompra;
    }

    [HttpPost("executar-compra")]
    [SwaggerOperation(
    Summary = "Motor de Compra",
    Description = "Realiza aportes dos clientes ativos, aplicando a cesta recomendada (Top Five) e distribuindo os ativos proporcionalmente para cada custódia filhote..")]
    public async Task<IActionResult> ExecutarCompra([FromBody] ExecutarCompraRequest request)
    {
        try
        {
            var resultado = await _motorCompra.ExecutarCompraAsync(request.DataReferencia);

            var ordensAgrupadas = resultado.OrdensExecutadas
                .GroupBy(o => o.Ticker)
                .Select(g => new OrdemCompraDto
                {
                    Ticker = g.Key.Replace("F", ""),
                    QuantidadeTotal = g.Sum(o => o.Quantidade),
                    PrecoUnitario = g.First().PrecoUnitario,
                    ValorTotal = g.Sum(o => o.Quantidade * o.PrecoUnitario),
                    Detalhes = g.Select(o => new DetalheOrdemDto
                    {
                        Tipo = o.TipoMercado.ToString().ToUpper(),
                        Ticker = o.TipoMercado == TipoMercado.Fracionario
                            ? o.Ticker + "F"
                            : o.Ticker,
                        Quantidade = o.Quantidade
                    }).ToList()
                }).ToList();

            var distribuicoesAgrupadas = resultado.DistribuicoesPorCliente
                .Select(d => new DistribuicaoClienteDto
                {
                    ClienteId = d.ClienteId,
                    Nome = d.Nome,
                    ValorAporte = d.ValorAporte,
                    Ativos = d.AtivosPorTicker.Select(a => new AtivoDistribuicaoDto
                    {
                        Ticker = a.Key,
                        Quantidade = a.Value
                    }).ToList()
                }).ToList();

            var response = new ExecutarCompraResponse
            {
                DataExecucao = resultado.DataExecucao,
                TotalClientes = resultado.DistribuicoesPorCliente.Count,
                TotalConsolidado = resultado.ValorTotalAportes,
                OrdensCompra = ordensAgrupadas,
                Distribuicoes = distribuicoesAgrupadas,
                ResiduosCustMaster = resultado.ResiduosPorTicker
                    .Select(r => new ResiduoDto
                    {
                        Ticker = r.Key,
                        Quantidade = r.Value
                    }).ToList(),
                EventosIRPublicados = resultado.EventosIR.Count,
                Mensagem = resultado.Mensagem
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}