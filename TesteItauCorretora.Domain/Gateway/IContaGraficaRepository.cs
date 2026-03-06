namespace TesteItauCorretora.Domain.Gateway;
public interface IContaGraficaRepository
{
    Task<Entities.ContaGrafica?> ObterContaMasterAsync();
    Task<Entities.ContaGrafica?> ObterPorIdAsync(int id);
    Task<Entities.ContaGrafica?> ObterPorClienteIdAsync(int clienteId); 
    Task AdicionarAsync(Entities.ContaGrafica conta);
    Task<string> GerarProximoNumeroContaAsync(Entities.TipoConta tipo);
}