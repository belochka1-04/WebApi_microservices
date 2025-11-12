using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.AspNetCore;

namespace SharedMicroserviceLibrary.Logging
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, config) =>
            {
                config.MinimumLevel.Debug()
                      .WriteTo.Console()
                      .WriteTo.File(
                        path: "logs/webapi-logs.txt",
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
            });
        }

        public static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration configuration)
        {
            configuration.ReadFrom.Configuration(context.Configuration.GetSection("Serilog"));
        }
    }
}
