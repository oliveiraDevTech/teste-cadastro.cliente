namespace Driven.RabbitMQ.Settings;

/// <summary>
/// Configurações do RabbitMQ
/// </summary>
public class RabbitMQSettings
{
    /// <summary>
    /// Nome do host do RabbitMQ
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Porta do RabbitMQ
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Senha
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Nome da virtual host
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Timeout de conexão em milissegundos
    /// </summary>
    public int ConnectionTimeout { get; set; } = 10000;

    /// <summary>
    /// Número máximo de tentativas de reconexão
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Delay entre tentativas em milissegundos
    /// </summary>
    public int RetryDelay { get; set; } = 1000;
}
