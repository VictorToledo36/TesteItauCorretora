namespace TesteItauCorretora.Domain.Exceptions;

public class CestaRecomendacaoInvalidaException : Exception
{
    public CestaRecomendacaoInvalidaException(string mensagem)
        : base(mensagem)
    {
    }
}
