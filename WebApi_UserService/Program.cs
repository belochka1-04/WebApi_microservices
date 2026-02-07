using KameraData.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Middleware;
using SharedMicroserviceLibrary.Logging;
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

// =====================================================
// 1. DATABASE CONFIGURATION
// =====================================================
string connectionString = builder.Configuration.GetConnectionString("KameraDb")
    ?? throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");

builder.Services.AddDbContext<KameraDbContext>(options =>
    options.UseSqlServer(connectionString));

// =====================================================
// 2. APPLICATION SERVICES
// =====================================================
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

// =====================================================
// 3. MASSTRANSIT (RabbitMQ)
// =====================================================
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

// =====================================================
// 4. SHARED SERVICES (Controllers, Swagger, JSON)
// =====================================================
builder.Services.AddCustomServices(
    builder.Configuration,
    apiTitle: "User Service API",
    apiVersion: "v1",
    addJwtToSwagger: true);

// =====================================================
// 5. JWT AUTHENTICATION
// =====================================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// =====================================================
// 6. LOGGING (Serilog)
// =====================================================
builder.Host.UseCustomSerilog();

// =====================================================
// 7. WINDOWS SERVICE SUPPORT
// =====================================================
builder.Host.UseWindowsService();

// =====================================================
// 8. PROMETHEUS METRICS
// =====================================================
builder.Services.UseHttpClientMetrics();

var app = builder.Build();

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================

// Swagger UI (только для dev/staging)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v1");
        c.RoutePrefix = string.Empty; // Swagger на корневом URL
    });
}

app.UseHttpsRedirection();

// Health check endpoint
app.UseHealthCheck();

// Request logging
app.UseRequestLogging();

// Authentication & Authorization (ПОРЯДОК ВАЖЕН!)
app.UseAuthentication();
app.UseAuthorization();

// Prometheus metrics
app.UseHttpMetrics();
app.MapMetrics("/metrics");

// Controllers
app.MapControllers();

// Graceful shutdown
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();