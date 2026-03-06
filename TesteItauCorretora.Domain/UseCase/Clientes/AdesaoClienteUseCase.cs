using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Exceptions;
using TesteItauCorretora.Domain.DTOs.Request;
using TesteItauCorretora.Domain.DTOs.Response;

namespace TesteItauCorretora.Domain.UseCase.Clientes;

public class AdesaoClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IContaGraficaRepository _contaRepository;

    public AdesaoClienteUseCase(IClienteRepository clienteRepository, IContaGraficaRepository contaRepository)
    {
        _clienteRepository = clienteRepository;
        _contaRepository = contaRepository;
    }

    public async Task<AdesaoResponse> Execute(AdesaoRequest request)
    {
        // Validar valor mensal mínimo
        if (request.ValorMensal < 100)
        {
            throw new ClienteValorMensalInvalidoException(request.ValorMensal);
        }

        // Validar CPF duplicado
        var clienteExistente = await _clienteRepository.ObterPorCpfAsync(request.Cpf);
        if (clienteExistente != null)
        {
            throw new ClienteCpfDuplicadoException(request.Cpf);
        }

        var cliente = new Cliente(
            request.Nome,
            request.Cpf,
            request.Email,
            request.ValorMensal);

        await _clienteRepository.AdicionarAsync(cliente);

        var numeroConta = await _contaRepository.GerarProximoNumeroContaAsync(TipoConta.Filhote);
        var conta = new ContaGrafica(numeroConta, TipoConta.Filhote);
        conta.AssociarCliente(cliente.Id);
        await _contaRepository.AdicionarAsync(conta);

        return new AdesaoResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.CPF,
            Email = cliente.Email,
            ValorMensal = cliente.ValorMensal,
            Ativo = cliente.Ativo,
            DataAdesao = cliente.DataAdesao,
            ContaGrafica = new ContaGraficaDto
            {
                Id = conta.Id,
                NumeroConta = conta.NumeroConta,
                Tipo = conta.Tipo.ToString(),
                DataCriacao = conta.DataCriacao
            }
        };
    }
}