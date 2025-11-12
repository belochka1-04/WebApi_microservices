using KameraData.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedMicroserviceLibrary;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using WebApi_StockCredsService.Services;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("KameraDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");
}

// Регистрация DbContext с конкретной строкой подключения
builder.Services.AddDbContext<KameraDbContext>(options =>
    options.UseSqlServer(connectionString));

// Регистрация сервисов приложения, с внедрением конкретного DbContext
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// Регистрация кросс-сервиса: контроллеры, swagger, json
builder.Services.AddCustomServices(builder.Configuration, "Application Microservice API", "v1");

// JWT аутентификация, если нужно
builder.Services.AddSharedAuthentication(builder.Configuration);

// Логирование Serilog
builder.Host.UseCustomSerilog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHealthCheck();
app.UseRequestLogging();

app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
