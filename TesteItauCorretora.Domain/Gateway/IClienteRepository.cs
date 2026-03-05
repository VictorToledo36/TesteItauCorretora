namespace TesteItauCorretora.Domain.Gateway;

public interface IClienteRepository
{
    Task<IEnumerable<Entities.Cliente>> ObterClientesAtivosAsync();
    Task<Entities.Cliente?> ObterPorIdAsync(int id);
    Task<Entities.Cliente?> ObterPorCpfAsync(string cpf);
    Task<Entities.Cliente?> ObterPorEmailAsync(string email);
    Task AdicionarAsync(Entities.Cliente cliente);
    Task AtualizarAsync(Entities.Cliente cliente);
}
