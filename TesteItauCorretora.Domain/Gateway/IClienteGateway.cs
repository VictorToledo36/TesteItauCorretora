using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Core.Gateway
{
    interface IClienteGateway
    {
        Task<Cliente> ObterClientePorCpf(string cpf);
        Task<Cliente> ObterClientePorEmail(string email);
        Task<Cliente> ObterClientePorId(Guid id);
        Task CadastrarCliente(Cliente cliente);
        Task AtualizarCliente(Cliente cliente);
    }
}
