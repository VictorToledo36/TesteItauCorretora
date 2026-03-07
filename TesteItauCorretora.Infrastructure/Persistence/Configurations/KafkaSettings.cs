namespace TesteItauCorretora.Infrastructure.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string TopicoIR { get; set; } = string.Empty;
}