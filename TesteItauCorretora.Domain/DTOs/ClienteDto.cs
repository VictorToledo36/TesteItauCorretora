using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Domain.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataAdesao { get; set; }

    public static ClienteDto FromEntity(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF,
            Email = cliente.Email,
            ValorMensal = cliente.ValorMensal,
            Ativo = cliente.Ativo,
            DataAdesao = cliente.DataAdesao
        };
    }
}