using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public class CestaRecomendacao
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataDesativacao { get; set; }

    public ICollection<ItemCesta>? Itens { get; set; }
}
