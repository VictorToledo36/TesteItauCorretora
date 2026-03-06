using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteItauCorretora.Domain.Entities;
using TesteItauCorretora.Domain.Gateway;

namespace TesteItauCorretora.Infrastructure.Gateway
{
    public class CotaHistGateway : ICotacaoRepository
    {
        private readonly string _pastaCotacoes;

        public CotaHistGateway(string pastaCotacoes = "cotacoes")
        {
            _pastaCotacoes = pastaCotacoes;
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<Cotacao?> ObterUltimaCotacaoAsync(string ticker)
        {
            return await Task.Run(() =>
            {
                var arquivos = ObterArquivosCotahist();
                
                foreach (var arquivo in arquivos)
                {
                    var cotacao = ParseArquivo(arquivo)
                        .Where(c => c.Ticker.Equals(ticker, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
                    
                    if (cotacao != null)
                        return cotacao;
                }
                
                return null;
            });
        }
        public async Task<Dictionary<string, Cotacao>> ObterUltimasCotacoesAsync(IEnumerable<string> tickers)
        {
            return await Task.Run(() =>
            {
                var resultado = new Dictionary<string, Cotacao>(StringComparer.OrdinalIgnoreCase);
                var tickersRestantes = new HashSet<string>(tickers, StringComparer.OrdinalIgnoreCase);
                var arquivos = ObterArquivosCotahist();
                
                foreach (var arquivo in arquivos)
                {
                    if (tickersRestantes.Count == 0)
                        break;
                    
                    var cotacoes = ParseArquivo(arquivo);
                    
                    foreach (var cotacao in cotacoes)
                    {
                        if (tickersRestantes.Contains(cotacao.Ticker) && !resultado.ContainsKey(cotacao.Ticker))
                        {
                            resultado[cotacao.Ticker] = cotacao;
                            tickersRestantes.Remove(cotacao.Ticker);
                        }
                    }
                }
                
                return resultado;
            });
        }
        private IEnumerable<string> ObterArquivosCotahist()
        {
            if (!Directory.Exists(_pastaCotacoes))
                return Enumerable.Empty<string>();
            
            return Directory.GetFiles(_pastaCotacoes, "COTAHIST_D*.TXT")
                .OrderByDescending(f => f);
        }

        private IEnumerable<Cotacao> ParseArquivo(string caminhoArquivo)
        {
            var cotacoes = new List<Cotacao>();
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            
            foreach (var linha in File.ReadLines(caminhoArquivo, encoding))
            {
                if (linha.Length < 245)
                    continue;
                
                var tipoRegistro = linha.Substring(0, 2);
                if (tipoRegistro != "01")
                    continue;
                
                var tipoMercadoStr = linha.Substring(24, 3).Trim();
                if (!int.TryParse(tipoMercadoStr, out var tipoMercado))
                    continue;
                
                if (tipoMercado != 10 && tipoMercado != 20)
                    continue;
                
                var codigoBDI = linha.Substring(10, 2).Trim();
                
                if (codigoBDI != "02" && codigoBDI != "96")
                    continue;
                
                try
                {
                    var cotacao = new Cotacao
                    {
                        DataPregao = DateTime.ParseExact(
                            linha.Substring(2, 8), 
                            "yyyyMMdd", 
                            CultureInfo.InvariantCulture),
                        Ticker = linha.Substring(12, 12).Trim(),
                        PrecoAbertura = ParsePreco(linha.Substring(56, 13)),
                        PrecoMaximo = ParsePreco(linha.Substring(69, 13)),
                        PrecoMinimo = ParsePreco(linha.Substring(82, 13)),
                        PrecoFechamento = ParsePreco(linha.Substring(108, 13))
                    };
                    
                    cotacoes.Add(cotacao);
                }
                catch
                {
                    continue;
                }
            }
            
            return cotacoes;
        }

        private decimal ParsePreco(string valorBruto)
        {
            if (long.TryParse(valorBruto.Trim(), out var valor))
                return valor / 100m;
            return 0m;
        }
    }
}
