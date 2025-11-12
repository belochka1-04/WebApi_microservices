using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SharedMicroserviceLibrary.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Incoming request: {method} {url} from {ip}",
                request.Method,
                $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation("Completed {method} {url} with status {statusCode} in {elapsed} ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // Опционально:
                // context.Response.Headers["X-Process-Time"] = stopwatch.ElapsedMilliseconds.ToString();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error during: {method} {url} after {elapsed} ms",
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
