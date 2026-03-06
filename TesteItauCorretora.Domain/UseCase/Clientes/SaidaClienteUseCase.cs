using TesteItauCorretora.Domain.DTOs.Response;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Core.UseCase.Clientes;
public class SaidaClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public SaidaClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<SaidaClienteResponse> Execute(int clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId);

        cliente.Sair();
        await _clienteRepository.AtualizarAsync(cliente);

        return new SaidaClienteResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Ativo = cliente.Ativo,
            DataSaida = cliente.DataSaida!.Value,
            Mensagem = "Adesao encerrada. Sua posicao em custodia foi mantida."
        };
    }
}