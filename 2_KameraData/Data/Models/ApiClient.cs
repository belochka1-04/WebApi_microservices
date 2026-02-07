public class ApiClient
{
    public int Id { get; set; }

    /// <summary>
    /// Публичный идентификатор клиента (client_id)
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// Хеш секрета клиента (client_secret), НЕ хранить в открытом виде
    /// </summary>
    public string ClientSecretHash { get; set; } = null!;

    /// <summary>
    /// Человеко‑читаемое имя клиента
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Разрешённые scopes (через пробел или в виде строки JSON)
    /// </summary>
    public string? AllowedScopes { get; set; }

    /// <summary>
    /// Активен ли клиент
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Разрешённые IP (опционально)
    /// </summary>
    public string? AllowedIps { get; set; }

    /// <summary>
    /// Разрешённые Origins (опционально)
    /// </summary>
    public string? AllowedOrigins { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
    public string? LastUpdatedBy { get; set; }
}