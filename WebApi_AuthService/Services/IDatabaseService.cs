using KameraData.Data.Models;

namespace WebApi_AuthService.Services
{
	public interface IDatabaseService
	{
		Task<bool> IsDatabaseHealthyAsync();

		Task<ApiClient?> GetApiClientByIdAsync(string clientId);

        // ========================================
        // AUTHENTICATION - Методы для JWT
        // ========================================

        /// <summary>
        /// Проверить credentials пользователя (для логина)
        /// </summary>
        /// <returns>User если credentials верные, null если неверные</returns>
        Task<User?> ValidateUserCredentialsAsync(string login, string password);

        /// <summary>
        /// Получить роли пользователя
        /// </summary>
        Task<string[]> GetUserRolesAsync(int userId);

        /// <summary>
        /// Проверить, имеет ли пользователь определенную роль
        /// </summary>
        Task<bool> UserHasRoleAsync(int userId, string role);

        /// <summary>
        /// Изменить пароль пользователя
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    }
    // DTO для создания пользователя
    public record CreateUserRequest
    {
        public required string Login { get; init; }
        public required string Password { get; init; }
        public string? TelegramId { get; init; }
        public string? CrmLogin { get; init; }
    }

    // DTO для обновления пользователя
    public record UpdateUserRequest
    {
        public string? Login { get; init; }
        public string? Password { get; init; }
        public string? TelegramId { get; init; }
        public int? DiagramProbability { get; init; }
        public int? ManualProbability { get; init; }
    }
}
