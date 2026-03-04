using TesteItauCorretora.Core.Gateway;
using TesteItauCorretora.Core.UseCase.Clientes;
using TesteItauCorretora.Core.UseCases.Clientes;
using TesteItauCorretora.Domain.Entities;

public class CriarClienteUseCase
{
    private readonly IClienteGateway _clienteRepository;
    private readonly IContaGraficaGateway _contaRepository;

    public async Task<ClienteDto> Execute(CriarClienteRequest request)
    {
        var cliente = new Cliente(
            request.Nome,
            request.CPF,
            request.Email,
            request.ValorMensal);

        await _clienteRepository.CadastrarCliente(cliente);

        var conta = ContaGrafica.CriarParaCliente(cliente.Id);
        await _contaRepository.Add(conta);

        return ClienteDto.FromEntity(cliente);
    }
}