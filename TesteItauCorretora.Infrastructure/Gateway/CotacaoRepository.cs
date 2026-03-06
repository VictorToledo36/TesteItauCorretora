using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Persistence;

namespace TesteItauCorretora.Infrastructure.Gateway
{
    public class CotacaoRepository : ICotacaoRepository
    {
        private readonly AppDbContext _context;
        private readonly CotaHistGateway _cotaHistGateway;

        public CotacaoRepository(AppDbContext context, string pastaCotacoes = "Cotacoes")
        {
            _context = context;
            _cotaHistGateway = new CotaHistGateway(pastaCotacoes);
        }

        public async Task<Cotacao?> ObterUltimaCotacaoAsync(string ticker)
        {
            // Primeiro tenta buscar no cache (banco)
            var cotacaoCache = await _context.Cotacoes
                .Where(c => c.Ticker == ticker)
                .OrderByDescending(c => c.DataPregao)
                .FirstOrDefaultAsync();

            if (cotacaoCache != null && cotacaoCache.DataPregao >= DateTime.Today.AddDays(-1))
            {
                return cotacaoCache;
            }

            var cotacaoArquivo = await _cotaHistGateway.ObterUltimaCotacaoAsync(ticker);
            
            if (cotacaoArquivo != null)
            {
                var existe = await _context.Cotacoes
                    .AnyAsync(c => c.Ticker == ticker && c.DataPregao == cotacaoArquivo.DataPregao);

                if (!existe)
                {
                    _context.Cotacoes.Add(cotacaoArquivo);
                    await _context.SaveChangesAsync();
                }

                return cotacaoArquivo;
            }

            return cotacaoCache;
        }

        public async Task<Dictionary<string, Cotacao>> ObterUltimasCotacoesAsync(IEnumerable<string> tickers)
        {
            var tickersList = tickers.ToList();
            var resultado = new Dictionary<string, Cotacao>(StringComparer.OrdinalIgnoreCase);

            // Primeiro busca no cache
            var cotacoesCache = await _context.Cotacoes
                .Where(c => tickersList.Contains(c.Ticker))
                .GroupBy(c => c.Ticker)
                .Select(g => g.OrderByDescending(c => c.DataPregao).First())
                .ToListAsync();

            foreach (var cotacao in cotacoesCache)
            {
                resultado[cotacao.Ticker] = cotacao;
            }

            var tickersParaBuscar = tickersList
                .Where(t => !resultado.ContainsKey(t) || 
                           resultado[t].DataPregao < DateTime.Today.AddDays(-1))
                .ToList();

            if (tickersParaBuscar.Any())
            {
                var cotacoesArquivo = await _cotaHistGateway.ObterUltimasCotacoesAsync(tickersParaBuscar);

                foreach (var kvp in cotacoesArquivo)
                {
                    var existe = await _context.Cotacoes
                        .AnyAsync(c => c.Ticker == kvp.Key && c.DataPregao == kvp.Value.DataPregao);

                    if (!existe)
                    {
                        _context.Cotacoes.Add(kvp.Value);
                    }

                    resultado[kvp.Key] = kvp.Value;
                }

                if (cotacoesArquivo.Any())
                {
                    await _context.SaveChangesAsync();
                }
            }

            return resultado;
        }

        public async Task ImportarCotacoesAsync(IEnumerable<string> tickers, DateTime? dataInicio = null)
        {
            var cotacoes = await _cotaHistGateway.ObterUltimasCotacoesAsync(tickers);

            foreach (var kvp in cotacoes)
            {
                if (dataInicio.HasValue && kvp.Value.DataPregao < dataInicio.Value)
                    continue;

                var existe = await _context.Cotacoes
                    .AnyAsync(c => c.Ticker == kvp.Key && c.DataPregao == kvp.Value.DataPregao);

                if (!existe)
                {
                    _context.Cotacoes.Add(kvp.Value);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
