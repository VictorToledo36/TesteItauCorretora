using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Domain.Gateway;

public interface IEventoIRPublisher
{
    Task PublicarAsync(EventoIR eventoIR, string cpf);

    /// Publica multiplos eventos de IR em paralelo 
    Task PublicarLoteAsync(IEnumerable<(EventoIR Evento, string CPF)> eventos);
}