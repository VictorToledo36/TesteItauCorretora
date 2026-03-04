using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Core.Gateway;

public interface IContaGraficaGateway
{
    Task Add(ContaGrafica conta);

    Task<ContaGrafica?> ObterPorId(int id);

    Task<ContaGrafica?> ObterPorClienteId(int clienteId);
}