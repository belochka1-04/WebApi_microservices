// SharedMicroserviceLibrary/ServiceCollectionExtensions.cs
using KameraData.SharedMicroservicesLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen; // <-- Добавлено
using Swashbuckle.AspNetCore.SwaggerUI;    // <-- Добавлено
using System.Text;
using KameraData.Data;

namespace SharedMicroserviceLibrary
{
    public static class ServiceCollectionExtensions
    {
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

        //public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // Add controllers and configure JSON options
        //    services.AddControllers()
        //        .AddJsonOptions(options =>
        //        {
        //            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        //            options.JsonSerializerOptions.MaxDepth = 10;
        //        });

        //    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //    services.AddEndpointsApiExplorer();
        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microservices API", Version = "v1" });

        //        // Добавьте настройку безопасности Swagger для JWT (если используете)
        //        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //        {
        //            Description = "JWT Authorization header using the Bearer scheme.",
        //            Name = "Authorization",
        //            In = ParameterLocation.Header,
        //            Type = SecuritySchemeType.Http,
        //            Scheme = "bearer",
        //            BearerFormat = "JWT"
        //        });
        //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                new OpenApiSecurityScheme
        //                {
        //                    Reference = new OpenApiReference
        //                    {
        //                        Type = ReferenceType.SecurityScheme,
        //                        Id = "Bearer"
        //                    }
        //                },
        //                new string[] {}
        //            }
        //        });
        //    });
        //    return services;
        //}


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


        public static IServiceCollection AddDatabaseService<T>(this IServiceCollection services, string connectionStringKey = "DefaultConnection") where T : class, IDatabaseService
        {
            services.AddScoped<IDatabaseService, T>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(connectionStringKey);
                // Use the service provider to resolve dependencies of T
                return ActivatorUtilities.CreateInstance<T>(provider, connectionString);
            });
            return services;
        }

        public static IServiceCollection AddDatabaseService<TInterface, TImplementation>(this IServiceCollection services, string connectionStringKey = "DefaultConnection")
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddScoped<TInterface, TImplementation>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(connectionStringKey);

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException($"Connection string '{connectionStringKey}' not found.");
                }

                // Assuming TImplementation has a constructor that takes a string (connection string) AND a DbContext
                // If not, you'll need to adjust this part
                var dbContext = provider.GetService<KameraDbContext>(); // Replace YourDbContext with the actual type

                if (dbContext != null)
                {
                    return ActivatorUtilities.CreateInstance<TImplementation>(provider, connectionString, dbContext);
                }
                else
                {
                    return ActivatorUtilities.CreateInstance<TImplementation>(provider, connectionString);
                }
            });
            return services;
        }
    }
}

