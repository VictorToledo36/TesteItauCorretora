namespace TesteItauCorretora.Domain.Exceptions;

public class ClienteNaoEncontradoException : Exception
{
    public ClienteNaoEncontradoException(int clienteId)
        : base($"Cliente com ID {clienteId} não encontrado.")
    {
    }
}