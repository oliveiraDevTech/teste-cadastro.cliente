namespace Driven.RabbitMQ.Interfaces;

/// <summary>
/// Interface para gerenciar a conexão e canal do RabbitMQ
/// </summary>
public interface IMessageBus : IDisposable
{
    /// <summary>
    /// Obtém a conexão com RabbitMQ
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Obtém o canal do RabbitMQ
    /// </summary>
    IModel Channel { get; }

    /// <summary>
    /// Verifica se está conectado
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Reconecta ao RabbitMQ
    /// </summary>
    /// <returns>True se conseguiu reconectar, false caso contrário</returns>
    bool TryConnect();
}
