using KameraData.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using SharedMicroserviceLibrary.Middleware;
using WebApi_GoogleSearchTemplatesService.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("KameraDb")
    ?? throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");

builder.Services.AddDbContext<KameraDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: new[] { 1205, 1222, 49918, 49919 });

        sqlOptions.CommandTimeout(30);
        sqlOptions.MaxBatchSize(1000);
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }));

builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

// HTTP client метрики (если будут исходящие запросы)
builder.Services.UseHttpClientMetrics();

// === Контроллеры + Swagger + JSON + JWT в Swagger ===
builder.Services.AddCustomServices(
    builder.Configuration,
    apiTitle: "Model Catalog Service API",
    apiVersion: "v1",
    addJwtToSwagger: true);   // как в UserService

// === JWT-аутентификация (принимаем токены AuthService) ===
builder.Services.AddJwtAuthentication(builder.Configuration); // как в UserService [web:18][web:27]

// Логирование Serilog
builder.Host.UseCustomSerilog();

var app = builder.Build();

// Swagger (можно по условию окружения, если хочешь)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Google Search Templates Service API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// HealthCheck + логирование запросов
app.UseHealthCheck();
app.UseRequestLogging();

// ВАЖНО: порядок
app.UseAuthentication();
app.UseAuthorization();   // авторизация после аутентификации [web:41][web:45]

// Prometheus HTTP-метрики
app.UseHttpMetrics();
app.MapMetrics("/metrics");

// Контроллеры
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
