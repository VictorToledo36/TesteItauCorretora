namespace TesteItauCorretora.Domain.Gateway;

public interface IEventoIRPublisher
{
    Task PublicarAsync(Entities.EventoIR eventoIR);
}
