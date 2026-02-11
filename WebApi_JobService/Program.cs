using KameraData.Data;
using KameraData.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Prometheus;
using Serilog;
using SharedMicroserviceLibrary.Authentication;
using SharedMicroserviceLibrary.Extensions;
using SharedMicroserviceLibrary.Logging;
using SharedMicroserviceLibrary.Middleware;
using WebApi_JobService;
using WebApi_JobService.Consumer.JobsService.Consumers;
using WebApi_JobService.Services;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
        ? AppContext.BaseDirectory
        : default
};

var builder = WebApplication.CreateBuilder(options);

// ===== 1. DB =====
var connectionString = builder.Configuration.GetConnectionString("KameraDb")
    ?? throw new InvalidOperationException("Connection string 'KameraDb' not found in configuration.");

builder.Services.AddDbContext<KameraDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// ===== 2. Application services =====
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

builder.Services.UseHttpClientMetrics();

// Controllers + Swagger + JSON
builder.Services.AddCustomServices(
    builder.Configuration,
    apiTitle: "Job Service API",
    apiVersion: "v1",
    addJwtToSwagger: true);

// JWT аутентификация (принимаем токены AuthService)
builder.Services.AddJwtAuthentication(builder.Configuration);

// HttpClient для UserService (при необходимости)
builder.Services.AddHttpClient<UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://your-userservice-host/"); // TODO: реальный URL UserService
});

// ===== 3. MassTransit / RabbitMQ =====
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<JobLinkedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");

        cfg.Host(
            rabbitSection.GetValue<string>("Host") ?? "rabbitmq",
            rabbitSection.GetValue<string>("VirtualHost") ?? "/",
            h =>
            {
                h.Username(rabbitSection.GetValue<string>("Username") ?? "guest");
                h.Password(rabbitSection.GetValue<string>("Password") ?? "guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

// ===== 4. Logging + Windows service =====
builder.Host.UseCustomSerilog();
builder.Host.UseWindowsService();

var app = builder.Build();

// ===== 5. Middleware pipeline =====

// Swagger
//if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseHealthCheck();
app.UseRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();
app.MapMetrics("/metrics");

app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
