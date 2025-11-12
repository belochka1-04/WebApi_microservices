using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen; // <-- Добавлено
using Swashbuckle.AspNetCore.SwaggerUI;    // <-- Добавлено
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace SharedMicroserviceLibrary.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // Пример: регистрация репозиториев, сервисов, кеша и прочего
        //    // services.AddScoped<IDatabaseService, DatabaseService>();

        //    // Можно добавить любые общие сервисы, которые нужны в каждом микросервисе
        //    return services;
        //}

        public static IServiceCollection AddSharedAuthentication(this IServiceCollection services, IConfiguration configuration, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            return services;
        }

        public static IServiceCollection AddSharedSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            return services;

        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Настройка аутентификации JWT
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]); // Получаем секретный ключ из конфигурации
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration, string apiTitle = "Microservice API", string apiVersion = "v1")
        {
            // Add controllers and configure JSON options
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.MaxDepth = 10;
                });

            // Swagger/OpenAPI registration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });

                // Optional JWT auth for Swagger, applies only if project uses it
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }

}
