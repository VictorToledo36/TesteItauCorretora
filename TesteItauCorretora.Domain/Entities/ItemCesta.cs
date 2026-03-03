using System;
using TesteItauCorretora.Domain.Entities;

namespace TesteItauCorretora.Domain.Entities
{
    public class ItemCesta
    {
        public int Id { get; set; }
        public int CestaRecomendacaoId { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public decimal Percentual { get; set; }
    }
}