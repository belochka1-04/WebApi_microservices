using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedMicroserviceLibrary.Authentication
{
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Добавляет JWT аутентификацию с настройками из конфигурации
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Читаем настройки из appsettings.json
            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException(
                    $"JWT settings not found in configuration. Please add '{JwtSettings.SectionName}' section.");

            // Валидация секретного ключа
            if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException(
                    "JWT SecretKey is missing or empty. Please configure it in appsettings.json");
            }

            if (jwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT SecretKey must be at least 32 characters long for security.");
            }

            // Регистрируем настройки в DI
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Убираем допуск времени
                };

                // Логирование ошибок аутентификации
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
