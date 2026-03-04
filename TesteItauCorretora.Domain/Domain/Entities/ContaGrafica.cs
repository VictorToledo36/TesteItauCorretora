
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteItauCorretora.Domain.Entities;

public enum TipoConta
{
    Master = 1,
    Filhote = 2
}

public class ContaGrafica
{
    public int Id { get; set; }
    public int ClienteID { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public TipoConta Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public ICollection<Custodia>? Custodias { get; set; }
}