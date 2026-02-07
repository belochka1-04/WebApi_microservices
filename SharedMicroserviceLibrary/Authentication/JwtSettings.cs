namespace SharedMicroserviceLibrary.Authentication
{
    /// <summary>
    /// Настройки JWT аутентификации
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>
        /// Секретный ключ для подписи токенов (минимум 32 символа)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Издатель токена (Issuer)
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Аудитория токена (Audience)
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Время жизни токена в минутах
        /// </summary>
        public int ExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// Требовать HTTPS в продакшене
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Валидировать Issuer
        /// </summary>
        public bool ValidateIssuer { get; set; } = false;

        /// <summary>
        /// Валидировать Audience
        /// </summary>
        public bool ValidateAudience { get; set; } = false;
    }
}