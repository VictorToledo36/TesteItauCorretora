using Microsoft.AspNetCore.Mvc;
using TesteItauCorretora.Core.UseCase.Clientes;
using TesteItauCorretora.Domain.DTOs.Request;
using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.UseCase.Clientes;

namespace TesteItauCorretora.Controllers.Clientes;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AdesaoClienteUseCase _adesaoUseCase;
    private readonly SaidaClienteUseCase _saidaUseCase;
    private readonly AlterarValorMensalUseCase _alterarValorMensalUseCase;
    private readonly ConsultarCarteiraUseCase _consultarCarteiraUseCase;
    private readonly ConsultarRentabilidadeUseCase _consultarRentabilidadeUseCase;

    public ClientesController(
        AdesaoClienteUseCase adesaoUseCase,
        SaidaClienteUseCase saidaUseCase,
        AlterarValorMensalUseCase alterarValorMensalUseCase,
        ConsultarCarteiraUseCase consultarCarteiraUseCase,
        ConsultarRentabilidadeUseCase consultarRentabilidadeUseCase)
    {
        _adesaoUseCase = adesaoUseCase;
        _saidaUseCase = saidaUseCase;
        _alterarValorMensalUseCase = alterarValorMensalUseCase;
        _consultarCarteiraUseCase = consultarCarteiraUseCase;
        _consultarRentabilidadeUseCase = consultarRentabilidadeUseCase;
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
            return BadRequest(new { erro = "CPF ja cadastrado no sistema.", codigo = "CLIENTE_CPF_DUPLICADO" });
        }
        catch (ClienteValorMensalInvalidoException)
        {
            return BadRequest(new { erro = "O valor mensal minimo e de R$ 100,00.", codigo = "VALOR_MENSAL_INVALIDO" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPost("{clienteId}/saida")]
    public async Task<ActionResult<SaidaClienteResponse>> Saida(int clienteId)
    {
        try
        {
            var response = await _saidaUseCase.Execute(clienteId);
            return Ok(response);
        }
        catch (ClienteNaoEncontradoException)
        {
            return NotFound(new { erro = "Cliente nao encontrado.", codigo = "CLIENTE_NAO_ENCONTRADO" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPut("{clienteId}/valor-mensal")]
    public async Task<ActionResult<AlterarValorMensalResponse>> AlterarValorMensal(
        int clienteId, [FromBody] AlterarValorMensalRequest request)
    {
        try
        {
            var response = await _alterarValorMensalUseCase.Execute(clienteId, request);
            return Ok(response);
        }
        catch (ClienteNaoEncontradoException)
        {
            return NotFound(new { erro = "Cliente nao encontrado.", codigo = "CLIENTE_NAO_ENCONTRADO" });
        }
        catch (ClienteValorMensalInvalidoException)
        {
            return BadRequest(new { erro = "O valor mensal minimo e de R$ 100,00.", codigo = "VALOR_MENSAL_INVALIDO" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("{clienteId}/carteira")]
    public async Task<ActionResult<CarteiraResponse>> ConsultarCarteira(int clienteId)
    {
        try
        {
            var response = await _consultarCarteiraUseCase.Execute(clienteId);
            return Ok(response);
        }
        catch (ClienteNaoEncontradoException)
        {
            return NotFound(new { erro = "Cliente nao encontrado.", codigo = "CLIENTE_NAO_ENCONTRADO" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("{clienteId}/rentabilidade")]
    public async Task<ActionResult<RentabilidadeResponse>> ConsultarRentabilidade(int clienteId)
    {
        try
        {
            var response = await _consultarRentabilidadeUseCase.Execute(clienteId);
            return Ok(response);
        }
        catch (ClienteNaoEncontradoException)
        {
            return NotFound(new { erro = "Cliente nao encontrado.", codigo = "CLIENTE_NAO_ENCONTRADO" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}