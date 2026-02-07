using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace SharedMicroserviceLibrary.Logging
{
    /// <summary>
    /// Extension методы для настройки Serilog
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Настраивает Serilog из конфигурации appsettings.json
        /// </summary>
        public static IHostBuilder UseCustomSerilog(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName);
            });
        }

        /// <summary>
        /// Настраивает Serilog программно с дефолтными настройками
        /// </summary>
        public static IHostBuilder UseCustomSerilog(
            this IHostBuilder builder,
            string logFilePath = "Logs/log-.txt",
            LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Is(minimumLevel)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                        theme: AnsiConsoleTheme.Code)
                    .WriteTo.File(
                        path: logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
            });
        }

        /// <summary>
        /// Создает базовый логгер для использования до инициализации DI
        /// </summary>
        public static void ConfigureBootstrapLogger(
            string logFilePath = "Logs/bootstrap-.txt",
            LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 3)
                .CreateBootstrapLogger();
        }
    }
}