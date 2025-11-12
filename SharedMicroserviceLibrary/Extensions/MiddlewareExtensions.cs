using Microsoft.AspNetCore.Builder;
using SharedMicroserviceLibrary.Middleware;

namespace SharedMicroserviceLibrary.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HealthCheckMiddleware>();
        }

    }
}
