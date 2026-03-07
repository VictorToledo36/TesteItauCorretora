using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TesteItauCorretora.Domain.DTOs;
using TesteItauCorretora.Domain.UseCase.Adm;
using TesteItauCorretora.Domain.UseCases;

namespace TesteItauCorretora.Controllers;

[ApiController]
[Route("api/admin")]
[Produces("application/json")]
[SwaggerTag("Gerenciamento do ADM — Cadastro das cestas, consulta histórico e conta - master")]
public class AdminController : ControllerBase
{
    private readonly CadastrarCestaUseCase _cadastrarCestaUseCase;
    private readonly ConsultarCestaAtualUseCase _consultarCestaAtualUseCase;
    private readonly ConsultarHistoricoCestasUseCase _consultarHistoricoCestasUseCase;
    private readonly ConsultarCustodiaMasterUseCase _consultarCustodiaMasterUseCase;

    public AdminController(
        CadastrarCestaUseCase cadastrarCestaUseCase,
        ConsultarCestaAtualUseCase consultarCestaAtualUseCase,
        ConsultarHistoricoCestasUseCase consultarHistoricoCestasUseCase,
        ConsultarCustodiaMasterUseCase consultarCustodiaMasterUseCase)
    {
        _cadastrarCestaUseCase = cadastrarCestaUseCase;
        _consultarCestaAtualUseCase = consultarCestaAtualUseCase;
        _consultarHistoricoCestasUseCase = consultarHistoricoCestasUseCase;
        _consultarCustodiaMasterUseCase = consultarCustodiaMasterUseCase;
    }

    [HttpPost("cesta")]
    [SwaggerOperation(
    Summary = "Cesta de tickers",
    Description = "Cadastra uma cesta de 5 ações.")]
    public async Task<IActionResult> CadastrarCesta([FromBody] CestaRequest request)
    {
        var response = await _cadastrarCestaUseCase.ExecutarAsync(request);
        return StatusCode(201, response);
    }

    [HttpGet("cesta/atual")]
    [SwaggerOperation(
    Summary = "Cesta Atual",
    Description = "Consulta cesta atual.")]
    public async Task<IActionResult> ConsultarCestaAtual()
    {
        var response = await _consultarCestaAtualUseCase.ExecutarAsync();
        if (response == null)
            return NotFound(new { erro = "Nenhuma cesta ativa encontrada.", codigo = "CESTA_NAO_ENCONTRADA" });
        return Ok(response);
    }

    [HttpGet("cesta/historico")]
    [SwaggerOperation(
    Summary = "Cesta Historico",
    Description = "Consulta histórico de cestas.")]
    public async Task<IActionResult> ConsultarHistorico()
    {
        var response = await _consultarHistoricoCestasUseCase.ExecutarAsync();
        return Ok(response);
    }

    [HttpGet("conta-master/custodia")]
    [SwaggerOperation(
    Summary = "Master Custodia",
    Description = "Consulta custodia da conta master.")]
    public async Task<IActionResult> ConsultarCustodiaMaster()
    {
        try
        {
            var response = await _consultarCustodiaMasterUseCase.ExecutarAsync();
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}