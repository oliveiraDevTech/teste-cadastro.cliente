namespace Core.Infra.Logging;

/// <summary>
/// Interface para logging centralizado da aplicação
/// </summary>
public interface IApplicationLogger
{
    /// <summary>
    /// Registra uma mensagem de informação
    /// </summary>
    void LogInformation(string message, params object?[] args);

    /// <summary>
    /// Registra uma mensagem de aviso
    /// </summary>
    void LogWarning(string message, params object?[] args);

    /// <summary>
    /// Registra uma mensagem de erro
    /// </summary>
    void LogError(string message, Exception? exception = null, params object?[] args);

    /// <summary>
    /// Registra uma mensagem de erro crítico
    /// </summary>
    void LogCritical(string message, Exception? exception = null, params object?[] args);

    /// <summary>
    /// Registra uma mensagem de debug
    /// </summary>
    void LogDebug(string message, params object?[] args);

    /// <summary>
    /// Registra o tempo de execução de uma operação
    /// </summary>
    void LogOperationDuration(string operationName, TimeSpan duration);
}
