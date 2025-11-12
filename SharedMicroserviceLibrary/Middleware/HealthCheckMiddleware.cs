using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SharedMicroserviceLibrary.Middleware
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthCheckMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _healthCheckEndpoint;
        private readonly string _databaseServiceType;

        public HealthCheckMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<HealthCheckMiddleware> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            _healthCheckEndpoint = _configuration["HealthCheckEndpoint"];
            if (string.IsNullOrWhiteSpace(_healthCheckEndpoint))
                _healthCheckEndpoint = "/healthcheck";
            _databaseServiceType = _configuration["DatabaseServiceType"];
            if (string.IsNullOrWhiteSpace(_databaseServiceType))
                _databaseServiceType = "IDatabaseService";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals(_healthCheckEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "application/json";

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    // Универсальный поиск реализованного IDatabaseService
                    var databaseService = scope.ServiceProvider.GetService(typeof(IDatabaseHealthCheck)) as IDatabaseHealthCheck;

                    if (databaseService == null)
                    {
                        await WriteResponseAsync(context, HttpStatusCode.NotImplemented,
                            new { status = "healthy", database = "not_available" });
                        return;
                    }

                    // Проверяем доступность базы
                    bool isDatabaseHealthy = await databaseService.IsDatabaseHealthyAsync();

                    if (isDatabaseHealthy)
                    {
                        await WriteResponseAsync(context, HttpStatusCode.OK,
                            new { status = "healthy", database = "ok" });
                    }
                    else
                    {
                        await WriteResponseAsync(context, HttpStatusCode.ServiceUnavailable,
                            new { status = "unhealthy", database = "connection_failed" });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Health check failed: {message}", ex.Message);
                    await WriteResponseAsync(context, HttpStatusCode.InternalServerError,
                        new { status = "unhealthy", error = ex.Message });
                }
            }
            else
            {
                await _next(context);
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object payload)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }

    //public static class HealthCheckMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<HealthCheckMiddleware>();
    //    }
    //}

    /// <summary>
    /// Контракт для проверки состояния базы, который можно реализовать в каждом микросервисе
    /// </summary>
    public interface IDatabaseHealthCheck
    {
        Task<bool> IsDatabaseHealthyAsync();
    }
}
