namespace TesteItauCorretora.Domain.Exceptions;

public class CestaAtivaJaExisteException : Exception
{
    public CestaAtivaJaExisteException()
        : base("Já existe uma cesta ativa no sistema. Desative a cesta atual antes de criar uma nova.")
    {
    }
}
