using KameraData.Data;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary;
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

string connectionString = builder.Configuration.GetConnectionString("KameraDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found in configuration.");
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
        // читаем настройки из appsettings.json -> секция "RabbitMq"
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitSection.GetValue<string>("Host") ?? "localhost";
        var vhost = rabbitSection.GetValue<string>("VirtualHost") ?? "/";
        var username = rabbitSection.GetValue<string>("Username") ?? "guest";
        var password = rabbitSection.GetValue<string>("Password") ?? "guest";

        cfg.Host(host, vhost, h =>
        {
            h.Username(username);
            h.Password(password);
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
builder.Host.UseWindowsService();

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
