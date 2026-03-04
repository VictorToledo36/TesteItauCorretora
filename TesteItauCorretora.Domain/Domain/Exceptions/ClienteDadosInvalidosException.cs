namespace TesteItauCorretora.Domain.Exceptions;

public class ClienteDadosInvalidosException : Exception
{
    public ClienteDadosInvalidosException(string mensagem)
        : base(mensagem)
    {
    }
}
