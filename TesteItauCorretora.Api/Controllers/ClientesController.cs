using Microsoft.AspNetCore.Mvc;
using TesteItauCorretora.Core.UseCase.Clientes;
using TesteItauCorretora.Domain.Exceptions;

namespace TesteItauCorretora.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AdesaoClienteUseCase _adesaoUseCase;

    public ClientesController(AdesaoClienteUseCase adesaoUseCase)
    {
        _adesaoUseCase = adesaoUseCase;
    }

    [HttpPost("adesao")]
    public async Task<ActionResult<AdesaoResponse>> Adesao([FromBody] AdesaoRequest request)
    {
        try
        {
            var response = await _adesaoUseCase.Execute(request);
            return StatusCode(201, response);
        }
        catch (ClienteCpfDuplicadoException)
        {
            return BadRequest(new 
            { 
                erro = "CPF ja cadastrado no sistema.",
                codigo = "CLIENTE_CPF_DUPLICADO"
            });
        }
        catch (ClienteValorMensalInvalidoException)
        {
            return BadRequest(new 
            { 
                erro = "O valor mensal minimo e de R$ 100,00.",
                codigo = "VALOR_MENSAL_INVALIDO"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
