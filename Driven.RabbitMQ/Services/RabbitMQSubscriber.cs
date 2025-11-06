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
                var messageJson = string.Empty;
                try
                {
                    var body = ea.Body.ToArray();
                    messageJson = Encoding.UTF8.GetString(body);
                    
                    Console.WriteLine($"[RabbitMQ] Mensagem recebida da fila '{queueName}': {messageJson.Substring(0, Math.Min(200, messageJson.Length))}...");
                    
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        Console.WriteLine($"[RabbitMQ] Mensagem deserializada com sucesso. Processando...");
                        await handler(message);
                        _messageBus.Channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine($"[RabbitMQ] Mensagem processada e confirmada (ACK)");
                    }
                    else
                    {
                        Console.WriteLine($"[RabbitMQ] ERRO: Mensagem deserializada como null");
                        _messageBus.Channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"[RabbitMQ] ERRO DE JSON: {jsonEx.Message}");
                    Console.WriteLine($"[RabbitMQ] JSON recebido: {messageJson}");
                    // Rejeita a mensagem e NÃO recoloca na fila (mensagem inválida)
                    _messageBus.Channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"[RabbitMQ] ERRO AO PROCESSAR: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[RabbitMQ] StackTrace: {ex.StackTrace}");
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
                var messageJson = string.Empty;
                try
                {
                    var body = ea.Body.ToArray();
                    messageJson = Encoding.UTF8.GetString(body);
                    
                    Console.WriteLine($"[RabbitMQ] Mensagem recebida da exchange '{exchangeName}': {messageJson.Substring(0, Math.Min(200, messageJson.Length))}...");
                    
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        Console.WriteLine($"[RabbitMQ] Mensagem deserializada com sucesso. Processando...");
                        await handler(message);
                        _messageBus.Channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine($"[RabbitMQ] Mensagem processada e confirmada (ACK)");
                    }
                    else
                    {
                        Console.WriteLine($"[RabbitMQ] ERRO: Mensagem deserializada como null");
                        _messageBus.Channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"[RabbitMQ] ERRO DE JSON: {jsonEx.Message}");
                    Console.WriteLine($"[RabbitMQ] JSON recebido: {messageJson}");
                    _messageBus.Channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"[RabbitMQ] ERRO AO PROCESSAR: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[RabbitMQ] StackTrace: {ex.StackTrace}");
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
