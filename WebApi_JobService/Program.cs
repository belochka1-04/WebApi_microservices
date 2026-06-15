using JobService.Application.Mappings;
using JobService.Infrastructure.Persistence;
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

builder.Services.AddDbContext<JobServiceDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddDbContextFactory<JobServiceDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// ===== 2. Application services =====
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDatabaseHealthCheck, DatabaseService>();

builder.Services.UseHttpClientMetrics();

//  gRPC 
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = !builder.Environment.IsProduction();
})
.AddJsonTranscoding();

// Для Swagger + gRPC UI
builder.Services.AddGrpcSwagger();
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
    var userServiceUrl = builder.Configuration["Services:UserService"]
        ?? "http://localhost:7050/";   // fallback

    client.BaseAddress = new Uri(userServiceUrl);

    // Опционально: таймауты и retry-политики
    client.Timeout = TimeSpan.FromSeconds(30);
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
// === Mapster Configuration ===
JobMappingConfig.Apply();

// ===== 4. Logging + Windows service =====
builder.Host.UseCustomSerilog();
builder.Host.UseWindowsService();

var app = builder.Build();

// ===== 5. Middleware pipeline 
app.UseRouting(); // для grpc

// 1️ Swagger (самый первый!)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Service API v1");
    c.RoutePrefix = string.Empty;
});

// 2️ Health/Metrics (до Auth)
app.UseHealthCheck();
app.UseHttpMetrics();

// 3️ Auth (JWT ДЛЯ gRPC + REST)
app.UseAuthentication();
app.UseAuthorization();

// 4️ Request logging (после Auth)
app.UseRequestLogging();

// 5️ HTTPS (последний, если нужен)
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

// 6️ gRPC
app.MapGrpcService<JobServiceImpl>();
// 7 Controllers 
app.MapControllers();
app.MapMetrics("/metrics");

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
