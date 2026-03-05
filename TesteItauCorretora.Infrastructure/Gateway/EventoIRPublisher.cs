using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Infrastructure.Gateway;

public class EventoIRPublisher : IEventoIRPublisher
{
    
    public async Task PublicarAsync(EventoIR eventoIR)
    {
        // Realizar publicação no Kafka
       
        
        await Task.CompletedTask;
    }
}
