// TesteItauCorretora/Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using TesteItauCorretora.Domain.DTOs;
using TesteItauCorretora.Domain.UseCase.Adm;

namespace TesteItauCorretora.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly CadastrarCestaUseCase _cadastrarCestaUseCase;

    public AdminController(CadastrarCestaUseCase cadastrarCestaUseCase)
    {
        _cadastrarCestaUseCase = cadastrarCestaUseCase;
    }

    [HttpPost("cesta")]
    public async Task<IActionResult> CadastrarCesta([FromBody] CestaRequest request)
    {
        var response = await _cadastrarCestaUseCase.ExecutarAsync(request);
        return StatusCode(201, response);
    }
}