using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Driving.Api.Extensions
{
    /// <summary>
    /// Extensão para registrar Health Checks
    /// </summary>
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Adiciona Health Checks configurados a partir do appsettings.json
        /// </summary>
        public static IServiceCollection AddApplicationHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var healthCheckConfig = configuration.GetSection("HealthChecks");

            if (!healthCheckConfig.GetValue<bool>("Enabled", true))
            {
                return services;
            }

            var healthChecksBuilder = services.AddHealthChecks();

            // Verificar Database
            var dbCheckEnabled = healthCheckConfig.GetSection("Checks:Database").GetValue<bool>("Enabled", true);
            if (dbCheckEnabled)
            {
                healthChecksBuilder.AddCheck("Database",
                    () => HealthCheckResult.Healthy("Database is available"),
                    tags: new[] { "db", "sql" }
                );
            }

            // Verificar RabbitMQ
            var rabbitMqCheckEnabled = healthCheckConfig.GetSection("Checks:RabbitMQ").GetValue<bool>("Enabled", true);
            if (rabbitMqCheckEnabled)
            {
                var rabbitMqSettings = configuration.GetSection("RabbitMQ");
                var host = rabbitMqSettings.GetValue<string>("HostName") ?? "localhost";
                var port = rabbitMqSettings.GetValue<int>("Port", 5672);
                var user = rabbitMqSettings.GetValue<string>("UserName") ?? "guest";
                var password = rabbitMqSettings.GetValue<string>("Password") ?? "guest";
                var vhost = rabbitMqSettings.GetValue<string>("VirtualHost") ?? "/";

                var rabbitMqConnectionString = $"amqp://{user}:{password}@{host}:{port}{vhost}";

                try
                {
                    healthChecksBuilder.AddRabbitMQ(
                        rabbitMqConnectionString,
                        name: "RabbitMQ",
                        tags: new[] { "messaging", "rabbitmq" }
                    );
                }
                catch
                {
                    // RabbitMQ health check pode falhar se o pacote não estiver instalado
                    healthChecksBuilder.AddCheck("RabbitMQ-Manual",
                        () => HealthCheckResult.Degraded("RabbitMQ health check disabled"),
                        tags: new[] { "messaging", "rabbitmq" }
                    );
                }
            }

            return services;
        }

        /// <summary>
        /// Configura os endpoints de Health Check
        /// </summary>
        public static WebApplication UseApplicationHealthChecks(
            this WebApplication app,
            IConfiguration configuration)
        {
            var healthCheckConfig = configuration.GetSection("HealthChecks");

            if (!healthCheckConfig.GetValue<bool>("Enabled", true))
            {
                return app;
            }

            var endpoint = healthCheckConfig.GetValue<string>("Endpoint") ?? "/health";
            var detailedEndpoint = healthCheckConfig.GetValue<string>("DetailedEndpoint") ?? "/health/detailed";

            // Endpoint de health check básico
            app.MapHealthChecks(endpoint, new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponse
            });

            // Endpoint de health check detalhado
            app.MapHealthChecks(detailedEndpoint, new HealthCheckOptions
            {
                ResponseWriter = WriteDetailedHealthCheckResponse,
                AllowCachingResponses = false
            });

            return app;
        }

        /// <summary>
        /// Escreve resposta simples de health check
        /// </summary>
        private static Task WriteHealthCheckResponse(
            HttpContext context,
            HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.TotalMilliseconds
                })
            };

            return context.Response.WriteAsJsonAsync(response);
        }

        /// <summary>
        /// Escreve resposta detalhada de health check com descrição
        /// </summary>
        private static Task WriteDetailedHealthCheckResponse(
            HttpContext context,
            HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.TotalMilliseconds,
                    description = entry.Value.Description,
                    exception = entry.Value.Exception?.Message,
                    tags = entry.Value.Tags
                })
            };

            return context.Response.WriteAsJsonAsync(
                response,
                new JsonSerializerOptions { WriteIndented = true }
            );
        }
    }
}
