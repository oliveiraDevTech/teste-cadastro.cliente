using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace Core.Infra;

/// <summary>
/// Extensão para registrar os serviços de infraestrutura no container de DI
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registra todos os serviços de infraestrutura
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Logging
        services.AddScoped<IApplicationLogger, ApplicationLogger>();

        // Cache - Registra MemoryCache e DistributedCache
        services.AddMemoryCache();
        services.AddDistributedMemoryCache(); // Registra IDistributedCache
        services.AddScoped<ICacheService, InMemoryCacheService>();

        // Email
        services.Configure<SmtpSettings>(options =>
            configuration.GetSection("Email:Smtp").Bind(options));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }

    /// <summary>
    /// Registra apenas o serviço de logging
    /// </summary>
    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddScoped<IApplicationLogger, ApplicationLogger>();
        return services;
    }

    /// <summary>
    /// Registra apenas o serviço de cache em memória
    /// </summary>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache(); // Registra IDistributedCache
        services.AddScoped<ICacheService, InMemoryCacheService>();
        return services;
    }

    /// <summary>
    /// Registra apenas o serviço de email
    /// </summary>
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(options =>
            configuration.GetSection("Email:Smtp").Bind(options));
        services.AddScoped<IEmailService, SmtpEmailService>();
        return services;
    }
}
