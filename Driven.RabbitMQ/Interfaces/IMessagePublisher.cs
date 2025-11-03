namespace Driven.RabbitMQ.Interfaces;

/// <summary>
/// Interface para publicar mensagens no RabbitMQ
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem em uma fila específica
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="queueName">Nome da fila</param>
    /// <param name="message">Mensagem a publicar</param>
    /// <returns>Task assíncrona</returns>
    Task PublishAsync<T>(string queueName, T message) where T : class;

    /// <summary>
    /// Publica uma mensagem em uma exchange específica
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="exchangeName">Nome da exchange</param>
    /// <param name="routingKey">Chave de roteamento</param>
    /// <param name="message">Mensagem a publicar</param>
    /// <returns>Task assíncrona</returns>
    Task PublishAsync<T>(string exchangeName, string routingKey, T message) where T : class;
}
