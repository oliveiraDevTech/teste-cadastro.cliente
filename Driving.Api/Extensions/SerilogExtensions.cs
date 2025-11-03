using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Driving.Api.Extensions
{
    /// <summary>
    /// Extensão para configurar Serilog
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Configura Serilog com base no appsettings.json
        /// </summary>
        public static IHostBuilder UseSerilogConfiguration(
            this IHostBuilder hostBuilder,
            IConfiguration configuration)
        {
            var serilogConfig = configuration.GetSection("Logging:Serilog");

            return hostBuilder.UseSerilog((context, loggerConfig) =>
            {
                // Obter nível mínimo de log
                var minimumLevel = Enum.Parse<LogEventLevel>(
                    serilogConfig.GetValue<string>("MinimumLevel", "Information")
                );

                loggerConfig
                    .MinimumLevel.Is(minimumLevel)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", context.Configuration["ApplicationSettings:ServiceName"] ?? "API")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

                // Configurar sinks (destinos de log)
                var writeTo = serilogConfig.GetSection("WriteTo");
                foreach (var sink in writeTo.GetChildren())
                {
                    var sinkName = sink.GetValue<string>("Name");
                    switch (sinkName?.ToLower())
                    {
                        case "console":
                            ConfigureConsoleSink(loggerConfig, sink);
                            break;
                        case "file":
                            ConfigureFileSink(loggerConfig, sink);
                            break;
                    }
                }

                // Configurar exceções não tratadas
                loggerConfig.Destructure.ToMaximumDepth(4)
                    .Destructure.ToMaximumStringLength(100)
                    .Destructure.ToMaximumCollectionCount(10);
            });
        }

        /// <summary>
        /// Configura sink de console
        /// </summary>
        private static void ConfigureConsoleSink(
            LoggerConfiguration loggerConfig,
            IConfigurationSection sinkConfig)
        {
            var outputTemplate = sinkConfig.GetValue<string>(
                "Args:outputTemplate",
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            );

            loggerConfig.WriteTo.Console(
                outputTemplate: outputTemplate
            );
        }

        /// <summary>
        /// Configura sink de arquivo
        /// </summary>
        private static void ConfigureFileSink(
            LoggerConfiguration loggerConfig,
            IConfigurationSection sinkConfig)
        {
            var path = sinkConfig.GetValue<string>("Args:path", "logs/app-.txt");
            var rollingInterval = Enum.Parse<RollingInterval>(
                sinkConfig.GetValue<string>("Args:rollingInterval", "Day")
            );
            var outputTemplate = sinkConfig.GetValue<string>(
                "Args:outputTemplate",
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            );

            // Criar diretório de logs se não existir
            var logDir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            loggerConfig.WriteTo.File(
                path: path,
                rollingInterval: rollingInterval,
                outputTemplate: outputTemplate,
                fileSizeLimitBytes: 104857600, // 100 MB
                retainedFileCountLimit: 10
            );
        }
    }
}
