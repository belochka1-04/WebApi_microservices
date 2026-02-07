using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace SharedMicroserviceLibrary.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует стандартные сервисы для микросервиса:
        /// Controllers, JSON options, Swagger с поддержкой JWT
        /// </summary>
        public static IServiceCollection AddCustomServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string apiTitle = "Microservice API",
            string apiVersion = "v1",
            bool addJwtToSwagger = true)
        {
            // Контроллеры с настройкой JSON
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.MaxDepth = 32;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo
                {
                    Title = apiTitle,
                    Version = apiVersion,
                    Description = $"{apiTitle} - Microservice Architecture"
                });

                // JWT авторизация в Swagger (опционально)
                if (addJwtToSwagger)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
                            Array.Empty<string>()
                        }
                    });
                }
            });

            return services;
        }
    }
}