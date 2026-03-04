namespace TesteItauCorretora.Domain.Exceptions;

public class ClienteValorMensalInvalidoException : Exception
{
    public decimal ValorMensal { get; }

    public ClienteValorMensalInvalidoException(decimal valorMensal)
        : base($"O valor mensal ({valorMensal:C}) é menor que o mínimo permitido de R$100,00.")
    {
        ValorMensal = valorMensal;
    }
}