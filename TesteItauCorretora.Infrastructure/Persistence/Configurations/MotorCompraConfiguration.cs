namespace TesteItauCorretora.Infrastructure.Configuration;

public class MotorCompraConfig
{
    public bool Enabled { get; set; } = true;

    // Padrão: todo dia às 10h.
    // O job verifica internamente se é dia de execução (5, 15 ou 25).
    public string CronExpression { get; set; } = "0 0 10 * * *";
}