using KameraData.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using SharedMicroserviceLibrary.Middleware;
using UserService.Consumers;
using WebApi_UserService.Services;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
        ? AppContext.BaseDirectory
        : default
};

var builder = WebApplication.CreateBuilder(options);

// ================================
// 1. DATABASE CONFIGURATION
// ================================
var connectionString = builder.Configuration.GetConnectionString("KameraDb")
    ?? throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");

builder.Services.AddDbContext<KameraDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// ================================
// 2. APPLICATION SERVICES
// ================================
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

builder.Services.Configure<AuthClientOptions>(
    builder.Configuration.GetSection("AuthService"));
builder.Services.AddHttpClient<IAuthTokenProvider, AuthTokenProvider>();

// ================================
// 3. MASSTRANSIT (RabbitMQ)
// ================================
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<JobCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");

        cfg.Host(
            rabbitSection.GetValue<string>("Host") ?? "localhost",
            rabbitSection.GetValue<string>("VirtualHost") ?? "/",
            h =>
            {
                h.Username(rabbitSection.GetValue<string>("Username") ?? "guest");
                h.Password(rabbitSection.GetValue<string>("Password") ?? "guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

// ================================
// 4. CONTROLLERS + SWAGGER + JSON
// ================================
builder.Services.AddCustomServices(
    builder.Configuration,
    apiTitle: "User Service API",
    apiVersion: "v1",
    addJwtToSwagger: true);

// ================================
// 5. JWT AUTHENTICATION (принимаем токены AuthService)
// ================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// ================================
// 6. LOGGING (Serilog)
// ================================
builder.Host.UseCustomSerilog();

// ================================
// 7. WINDOWS SERVICE SUPPORT
// ================================
builder.Host.UseWindowsService();

// ================================
// 8. PROMETHEUS METRICS
// ================================
builder.Services.UseHttpClientMetrics();

var app = builder.Build();

// ================================
// MIDDLEWARE PIPELINE
// ================================

// Swagger UI
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v1");
        c.RoutePrefix = string.Empty; // Swagger на корне: http://localhost:7050/
    });
}

app.UseHttpsRedirection();

// Healthcheck
app.UseHealthCheck();

// Request logging
app.UseRequestLogging();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Prometheus
app.UseHttpMetrics();
app.MapMetrics("/metrics");

// Controllers
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
