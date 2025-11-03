using System.Text;
using System.Text.Json;
using Driven.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Driven.RabbitMQ.Services;

/// <summary>
/// Implementação de subscrição e consumo de mensagens do RabbitMQ
/// </summary>
public class RabbitMQSubscriber : IMessageSubscriber
{
    private readonly IMessageBus _messageBus;

    public RabbitMQSubscriber(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Subscreve a uma fila específica
    /// </summary>
    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
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

            // Define a qualidade de serviço (QoS) - prefetch de 1 mensagem por vez
            _messageBus.Channel.BasicQos(0, 1, false);

            // Cria o consumer
            var consumer = new AsyncEventingBasicConsumer(_messageBus.Channel);

            // Define o handler para processar mensagens
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        await handler(message);
                        _messageBus.Channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    // Rejeita a mensagem e a recoloca na fila
                    _messageBus.Channel.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }
            };

            // Inicia o consumo
            _messageBus.Channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumerTag: $"{queueName}-consumer",
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao subscrever à fila '{queueName}'", ex);
        }
    }

    /// <summary>
    /// Subscreve a uma exchange específica com topic routing
    /// </summary>
    public async Task SubscribeAsync<T>(string exchangeName, string exchangeType, string routingKey, Func<T, Task> handler) where T : class
    {
        try
        {
            // Declara a exchange se não existir
            _messageBus.Channel.ExchangeDeclare(
                exchange: exchangeName,
                type: exchangeType,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Cria uma fila exclusiva para este subscriber
            var queueName = _messageBus.Channel.QueueDeclare().QueueName;

            // Faz o binding da fila à exchange
            _messageBus.Channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            // Define a qualidade de serviço (QoS)
            _messageBus.Channel.BasicQos(0, 1, false);

            // Cria o consumer
            var consumer = new AsyncEventingBasicConsumer(_messageBus.Channel);

            // Define o handler para processar mensagens
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        await handler(message);
                        _messageBus.Channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    // Rejeita a mensagem e a recoloca na fila
                    _messageBus.Channel.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }
            };

            // Inicia o consumo
            _messageBus.Channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumerTag: $"{exchangeName}-{routingKey}-consumer",
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao subscrever à exchange '{exchangeName}' com routing key '{routingKey}'", ex);
        }
    }
}
