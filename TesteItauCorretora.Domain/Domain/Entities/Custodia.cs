using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class Custodia
{
    public int Id { get; set; }
    public int ContaGraficaId { get; set; }
    public ContaGrafica ContaGrafica { get; private set; } = null!;
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
    public ICollection<Distribuicao> Distribuicao { get; set; } = new List<Distribuicao>();

    public void AdicionarAtivos(int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        var valorTotal = (Quantidade * PrecoMedio) + (quantidade * precoUnitario);
        Quantidade += quantidade;
        PrecoMedio = valorTotal / Quantidade;
        DataUltimaAtualizacao = DateTime.Now;
    }

    public void RemoverAtivos(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        if (quantidade > Quantidade)
            throw new InvalidOperationException("Quantidade insuficiente em custódia.");

        Quantidade -= quantidade;
        DataUltimaAtualizacao = DateTime.Now;
    }
}
