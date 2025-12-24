using KameraData.Data;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedMicroserviceLibrary;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using SharedMicroserviceLibrary.Middleware;
using UserService.Consumers;
using WebApi_UserService.Services;
using Prometheus;

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
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();
builder.Services.UseHttpClientMetrics(); // необязательно, но полезно

// 2. MassTransit с CONSUMER
builder.Services.AddMassTransit(x =>
{
    // Регистрируем Consumer (обработчик событий)
    x.AddConsumer<JobCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // ✅ Автоматически создаёт очередь для JobCreatedEvent
        cfg.ConfigureEndpoints(context);
    });
});

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
// Метрики HTTP-запросов
app.UseHttpMetrics();

// endpoint для метрик
app.MapMetrics("/metrics"); // здесь Prometheus будет их снимать
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
