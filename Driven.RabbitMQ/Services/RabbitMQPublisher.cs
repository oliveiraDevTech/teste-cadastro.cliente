using System.Text;
using System.Text.Json;
using Driven.RabbitMQ.Interfaces;
using RabbitMQ.Client;

namespace Driven.RabbitMQ.Services;

/// <summary>
/// Implementação de publicação de mensagens no RabbitMQ
/// </summary>
public class RabbitMQPublisher : IMessagePublisher
{
    private readonly IMessageBus _messageBus;

    public RabbitMQPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Publica uma mensagem em uma fila (queue)
    /// </summary>
    public async Task PublishAsync<T>(string queueName, T message) where T : class
    {
        try
        {
            // Declara a fila se não existir
            _messageBus.Channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serializa a mensagem
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Define propriedades da mensagem
            var properties = _messageBus.Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Publica a mensagem
            _messageBus.Channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao publicar mensagem na fila '{queueName}'", ex);
        }
    }

    /// <summary>
    /// Publica uma mensagem em uma exchange com chave de roteamento
    /// </summary>
    public async Task PublishAsync<T>(string exchangeName, string routingKey, T message) where T : class
    {
        try
        {
            // Declara a exchange se não existir
            _messageBus.Channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Serializa a mensagem
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Define propriedades da mensagem
            var properties = _messageBus.Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Publica a mensagem
            _messageBus.Channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao publicar mensagem na exchange '{exchangeName}' com routing key '{routingKey}'", ex);
        }
    }
}
