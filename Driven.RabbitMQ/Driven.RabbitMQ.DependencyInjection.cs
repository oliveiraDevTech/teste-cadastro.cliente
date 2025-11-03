using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Driven.RabbitMQ.Interfaces;
using Driven.RabbitMQ.Services;
using Driven.RabbitMQ.Settings;

namespace Driven.RabbitMQ;

/// <summary>
/// Extensão para registrar os serviços de RabbitMQ no container de DI
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registra os serviços do RabbitMQ no container de DI
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddRabbitMQServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registra as configurações do RabbitMQ
        var rabbitMqSection = configuration.GetSection("RabbitMQ");
        if (!rabbitMqSection.Exists())
            rabbitMqSection = configuration.GetSection("MessageQueue");

        services.Configure<RabbitMQSettings>(options =>
            rabbitMqSection.Bind(options));

        // Registra a factory de conexão como Singleton (uma única conexão)
        services.AddSingleton<IMessageBus, RabbitMQConnectionFactory>();

        // Registra o publisher e subscriber como Scoped
        services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
        services.AddScoped<IMessageSubscriber, RabbitMQSubscriber>();

        return services;
    }
}
