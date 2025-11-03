using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

/// <summary>
/// Extensão para configurar a injeção de dependências de Core.Application
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adiciona os serviços de aplicação ao container de DI
    /// </summary>
    /// <param name="services">Container de serviços</param>
    /// <param name="jwtSecret">Chave secreta para JWT</param>
    /// <param name="jwtIssuer">Emissor do JWT (opcional)</param>
    /// <param name="jwtAudience">Audiência do JWT (opcional)</param>
    /// <param name="jwtExpirationMinutes">Tempo de expiração do JWT em minutos (opcional)</param>
    /// <returns>IServiceCollection para encadeamento</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        string jwtSecret,
        string jwtIssuer = "CadastroClientesApi",
        string jwtAudience = "CadastroClientesApp",
        int jwtExpirationMinutes = 60)
    {
        // Registrar os serviços de aplicação
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IAuthenticationService>(provider =>
            new AuthenticationService(jwtSecret, jwtIssuer, jwtAudience, jwtExpirationMinutes)
        );

        return services;
    }
}
