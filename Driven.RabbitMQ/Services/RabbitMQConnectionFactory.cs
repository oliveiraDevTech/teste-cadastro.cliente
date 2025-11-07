using Microsoft.Extensions.Options;
using Driven.RabbitMQ.Interfaces;
using Driven.RabbitMQ.Settings;
using RabbitMQ.Client;

namespace Driven.RabbitMQ.Services;

/// <summary>
/// Factory para gerenciar conexões e canais do RabbitMQ
/// </summary>
public class RabbitMQConnectionFactory : IMessageBus
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;
    private int _retryCount;

    public IConnection Connection => _connection ?? throw new InvalidOperationException("Não conectado ao RabbitMQ");
    public IModel Channel => _channel ?? throw new InvalidOperationException("Canal não disponível");
    public bool IsConnected => _connection?.IsOpen ?? false;

    public RabbitMQConnectionFactory(IOptions<RabbitMQSettings> options)
    {
        _settings = options.Value;
        _retryCount = 0;
    }

    /// <summary>
    /// Tenta conectar ao RabbitMQ com retry automático
    /// </summary>
    public bool TryConnect()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                RequestedConnectionTimeout = TimeSpan.FromMilliseconds(_settings.ConnectionTimeout),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _retryCount = 0;

            return IsConnected;
        }
        catch (Exception ex)
        {
            _retryCount++;

            if (_retryCount < _settings.MaxRetries)
            {
                System.Threading.Thread.Sleep(_settings.RetryDelay);
                return TryConnect();
            }

            throw new InvalidOperationException(
                $"Falha ao conectar ao RabbitMQ após {_settings.MaxRetries} tentativas. " +
                $"Host: {_settings.HostName}:{_settings.Port}", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
        catch { }
    }
}
