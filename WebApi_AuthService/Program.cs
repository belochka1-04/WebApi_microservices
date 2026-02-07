using KameraData.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using SharedMicroserviceLibrary.Middleware;
using WebApi_AuthService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();
// ================================
// 1. DATABASE CONFIGURATION
// ================================
var connectionString = builder.Configuration.GetConnectionString("KameraDb")
    ?? throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");

builder.Services.AddDbContext<KameraDbContext>(options =>
    options.UseSqlServer(connectionString));

// ================================
// 2. JWT SETTINGS (общий JwtSettings из shared)
// ================================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// ================================
// 3. APPLICATION SERVICES
// ================================
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IClientTokenService, ClientTokenService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

builder.Services.UseHttpClientMetrics();

// ================================
// 4. CONTROLLERS + SWAGGER
// ================================
builder.Services.AddCustomServices(
    builder.Configuration,
    apiTitle: "Auth Service API",
    apiVersion: "v1",
    addJwtToSwagger: false); // тут токен не принимаем, только выдаём

// ================================
// 5. LOGGING (Serilog)
// ================================
builder.Host.UseCustomSerilog();

var app = builder.Build();

// ================================
// PIPELINE
// ================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Health / logging
app.UseHealthCheck();
app.UseRequestLogging();

// Метрики HTTP-запросов
app.UseHttpMetrics();
app.MapMetrics("/metrics");

// Маршрутизация контроллеров
app.MapControllers();

// Корректное завершение логгера
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
