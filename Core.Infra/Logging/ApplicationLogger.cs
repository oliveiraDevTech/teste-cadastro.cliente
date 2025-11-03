namespace Core.Infra.Logging;

/// <summary>
/// Implementação de logging usando ILogger padrão do .NET
/// </summary>
public class ApplicationLogger : IApplicationLogger
{
    private readonly ILogger<ApplicationLogger> _logger;

    public ApplicationLogger(ILogger<ApplicationLogger> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object?[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, Exception? exception = null, params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogCritical(string message, Exception? exception = null, params object?[] args)
    {
        _logger.LogCritical(exception, message, args);
    }

    public void LogDebug(string message, params object?[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogOperationDuration(string operationName, TimeSpan duration)
    {
        var durationMs = duration.TotalMilliseconds;
        var level = durationMs > 1000 ? "Aviso" : "Informação";
        LogInformation($"{level}: Operação '{operationName}' completada em {durationMs:F2}ms");
    }
}
