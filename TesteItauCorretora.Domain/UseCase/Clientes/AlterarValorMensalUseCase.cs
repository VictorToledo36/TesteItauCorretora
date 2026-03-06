using TesteItauCorretora.Domain.DTOs.Request;
using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Core.UseCase.Clientes;
public class AlterarValorMensalUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public AlterarValorMensalUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<AlterarValorMensalResponse> Execute(int clienteId, AlterarValorMensalRequest request)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        var valorAnterior = cliente.ValorMensal;
        cliente.AlterarValorMensal(request.NovoValorMensal);
        await _clienteRepository.AtualizarAsync(cliente);

        return new AlterarValorMensalResponse
        {
            ClienteId = cliente.Id,
            ValorMensalAnterior = valorAnterior,
            ValorMensalNovo = cliente.ValorMensal,
            DataAlteracao = DateTime.Now,
            Mensagem = "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra."
        };
    }
}