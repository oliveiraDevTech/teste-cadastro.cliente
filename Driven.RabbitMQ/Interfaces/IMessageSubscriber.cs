namespace Driven.RabbitMQ.Interfaces;

/// <summary>
/// Interface para subscrever e consumir mensagens do RabbitMQ
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Subscreve a uma fila específica
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="queueName">Nome da fila</param>
    /// <param name="handler">Função para processar a mensagem</param>
    /// <returns>Task assíncrona</returns>
    Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class;

    /// <summary>
    /// Subscreve a uma exchange específica
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="exchangeName">Nome da exchange</param>
    /// <param name="exchangeType">Tipo da exchange (direct, fanout, topic, headers)</param>
    /// <param name="routingKey">Chave de roteamento</param>
    /// <param name="handler">Função para processar a mensagem</param>
    /// <returns>Task assíncrona</returns>
    Task SubscribeAsync<T>(string exchangeName, string exchangeType, string routingKey, Func<T, Task> handler) where T : class;
}
