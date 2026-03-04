namespace TesteItauCorretora.Domain.Exceptions;

public class ClienteCpfDuplicadoException : Exception
{
    public string CPF { get; }

    public ClienteCpfDuplicadoException(string cpf)
        : base($"Já existe um cliente cadastrado com o CPF {cpf}.")
    {
        CPF = cpf;
    }
}
