namespace TesteItauCorretora.Domain.Gateway;

public interface IOrdemCompraRepository
{
    Task AdicionarAsync(Entities.OrdemCompra ordem);
    Task<IEnumerable<Entities.OrdemCompra>> ObterPorDataAsync(DateTime data);
}
