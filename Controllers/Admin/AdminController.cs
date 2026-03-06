using Microsoft.AspNetCore.Mvc;
using TesteItauCorretora.Domain.DTOs;
using TesteItauCorretora.Domain.UseCase.Adm;
using TesteItauCorretora.Domain.UseCases;

namespace TesteItauCorretora.Controllers;

[ApiController]
[Route("api/admin")]
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
    public async Task<IActionResult> CadastrarCesta([FromBody] CestaRequest request)
    {
        var response = await _cadastrarCestaUseCase.ExecutarAsync(request);
        return StatusCode(201, response);
    }

    [HttpGet("cesta/atual")]
    public async Task<IActionResult> ConsultarCestaAtual()
    {
        var response = await _consultarCestaAtualUseCase.ExecutarAsync();
        if (response == null)
            return NotFound(new { erro = "Nenhuma cesta ativa encontrada.", codigo = "CESTA_NAO_ENCONTRADA" });
        return Ok(response);
    }

    [HttpGet("cesta/historico")]
    public async Task<IActionResult> ConsultarHistorico()
    {
        var response = await _consultarHistoricoCestasUseCase.ExecutarAsync();
        return Ok(response);
    }

    [HttpGet("conta-master/custodia")]
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