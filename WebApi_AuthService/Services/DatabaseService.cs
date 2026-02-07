using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_AuthService.Services;

namespace WebApi_AuthService.Services
{
    public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
    {
        private readonly KameraDbContext _dbContext;
        private readonly NLog.ILogger _logger;

        public DatabaseService(KameraDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<bool> IsDatabaseHealthyAsync()
        {
            try
            {
                return await _dbContext.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка проверки подключения к базе данных");
                return false;
            }
        }

		public async Task<ApiClient?> GetApiClientByIdAsync(string clientId)
		{
			try
			{
				return await _dbContext.ApiClients
					.AsNoTracking()
					.FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsActive);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения ApiClient для clientId={clientId}", clientId);
				throw;
			}
		}

        // ========================================
        // AUTHENTICATION - Методы для JWT
        // ========================================

        public async Task<User?> ValidateUserCredentialsAsync(string login, string password)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                {
                    _logger.Warn("User not found: {Login}", login);
                    return null;
                }

                // Проверяем пароль
                if (!VerifyPassword(password, user.Password))
                {
                    _logger.Warn("Invalid password for user: {Login}", login);
                    return null;
                }

                // Проверяем, активен ли пользователь
                if (user.Access == 0)
                {
                    _logger.Warn("User account is disabled: {Login}", login);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating credentials for: {Login}", login);
                throw;
            }
        }

        public async Task<string[]> GetUserRolesAsync(int userId)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return Array.Empty<string>();

                // Определяем роли на основе полей в User
                var roles = new List<string>();

                // Базовая роль
                roles.Add("User");

                // Админ (можно добавить поле IsAdmin в User или использовать Access)
                if (user.Access >= 10) // Например, Access >= 10 = Admin
                    roles.Add("Admin");

                // PRO пользователь
                if (user.ProUntil.HasValue && user.ProUntil.Value > DateTime.UtcNow)
                    roles.Add("Pro");

                return roles.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting roles for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UserHasRoleAsync(int userId, string role)
        {
            var roles = await GetUserRolesAsync(userId);
            return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                    return false;

                // Проверяем старый пароль
                if (!VerifyPassword(oldPassword, user.Password))
                {
                    _logger.Warn("Invalid old password for user: {UserId}", userId);
                    return false;
                }

                // Устанавливаем новый пароль
                user.Password = HashPassword(newPassword);
                user.LastUpdatedDateTime = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.Info("Password changed for user: {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }
        // ========================================
        // PRIVATE HELPERS - Хеширование паролей
        // ========================================

        /// <summary>
        /// Хеширует пароль с использованием BCrypt
        /// </summary>
        private string HashPassword(string password)
        {
            // ⚠️ ВАЖНО: Установите NuGet пакет BCrypt.Net-Next
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Проверяет пароль с хешем
        /// </summary>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Если пароль в БД не хеширован (legacy), сравниваем напрямую
                // ⚠️ ВРЕМЕННОЕ РЕШЕНИЕ - УДАЛИТЕ ПОСЛЕ МИГРАЦИИ ВСЕХ ПАРОЛЕЙ
                return password == hashedPassword;
            }
        }
    }
}
