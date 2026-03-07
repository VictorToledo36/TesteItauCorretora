using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TesteItauCorretora.Core.UseCase.Clientes;
using TesteItauCorretora.Domain.DTOs.Request;
using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.UseCase.Clientes;

namespace TesteItauCorretora.Controllers.Clientes;

[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
[SwaggerTag("Gerenciamento de clientes  adesao, saida, alteracao de valor mensal e consultas")]
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
    [SwaggerOperation(
    Summary = "Adesao de cliente",
    Description = "Cadastra um novo cliente no plano de investimento recorrente. O valor mensal minimo e R$ 100,00.")]
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
    [SwaggerOperation(
    Summary = "Saida de cliente",
    Description = "Desativa o cliente.")]
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
    [SwaggerOperation(
    Summary = "Valor Mensal",
    Description = "Altera valor mensal.")]
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
    [SwaggerOperation(
    Summary = "Carteira",
    Description = "Consulta a carteira do cliente.")]
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
    [SwaggerOperation(
    Summary = "Rentabilidade",
    Description = "Consulta a rentabilidade da carteira do cliente.")]
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