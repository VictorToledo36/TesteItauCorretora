namespace TesteItauCorretora.Domain.Gateway;
public interface IDistribuicaoRepository
{
    Task<IEnumerable<Entities.Distribuicao>> ObterPorCustodiaAsync(int custodiaFilhoteId);
    Task<IEnumerable<Entities.Distribuicao>> ObterPorContaAsync(int contaGraficaId);
}