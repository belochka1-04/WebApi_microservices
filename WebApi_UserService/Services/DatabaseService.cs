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
using WebApi_UserService.Services;

namespace WebApi_UserService.Services
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

        #region user_stocks

        public async Task<StockCred> GetStockCredById(int stockId)
        {
            try
            {
                // Используем LINQ для получения запасов пользователя
                var userStocks = (await _dbContext.UserStocks.Where(x => x.Id == stockId)
                    .Include(x => x.Stock)
                    .FirstOrDefaultAsync())?.Stock;

                return userStocks;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении запасов пользователя (userId: {stockId}): {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении запасов пользователя (userId: {stockId}): {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
        }
        public async Task<List<UserStock>> GetUserStocksByStockCredAsync(int stockCredId, int userId)
        {
            try
            {
                // Используем LINQ для получения запасов пользователя
                var result = await _dbContext.UserStocks
                            .Where(x => x.StockId == stockCredId && x.UserId == userId)
                            .ToListAsync();

                return result;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении stock creds): {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении stock creds): {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
        }

        public async Task<List<UserStock>> GetStockById(int userId)
        {
            try
            {
                // Получаем список stock_id, которые видит пользователь (свои + shared)
                var visibleStockIds = await _dbContext.UserStocks
                    .Where(us => us.UserId == userId)
                    .Select(us => us.StockId)
                    .Union(
                        _dbContext.SharedStocks
                            .Where(ss => ss.UserId == userId)
                            .Select(ss => ss.StockId)
                    )
                    .ToListAsync();

                // Получаем UserStocks для всех видимых stock_id (независимо от владельца)
                var userStocks = await _dbContext.UserStocks
                    .Include(us => us.Stock)
                    .Where(us => visibleStockIds.Contains(us.StockId))
                    .ToListAsync();

                return userStocks;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении видимых запасов пользователя (userId: {userId}): {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении видимых запасов пользователя (userId: {userId}): {ex.Message}");
                throw;
            }
        }

        public async Task InsertUserStockAsync(UserStock responce)
        {

            if (responce != null)
            {
                try
                {
                    _dbContext.UserStocks.Add(responce);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Error("Ошибка:" + ex);
                }
            }

        }

        public async Task DeleteUserStocksAsync(int Id)
        {
            try
            {
                // Получаем все записи JobStock с указанным jobId
                var jobStocks = await _dbContext.UserStocks
                    .Where(js => js.Id == Id)
                    .ToListAsync();

                // Удаляем найденные записи
                _dbContext.UserStocks.RemoveRange(jobStocks);

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при удалении userStocks для Id {Id}: {ex.Message}");
            }

        }
        #endregion

        #region user
        public async Task<int> GetUserCrmAsync(int UserId)
        {
            try
            {
                var user = await _dbContext.Users
                    .Where(u => u.Id == UserId)
                    .Select(u => u.CrmId)
                    .FirstOrDefaultAsync();
                return user.GetValueOrDefault();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении CRM_id для пользователя (userId: {UserId}): {ex.Message}");
                throw; // Пробрасываем исключение дальше, если нужно
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении CRM_id для пользователя (userId: {UserId}): {ex.Message}");
                throw;
            }
        }

        public async Task SaveDefaultUserToBDAsync(int? telegramId, string prefersTelegram)
        {
            try
            {
                // Проверяем, существует ли пользователь с таким telegramId
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

                if (existingUser != null)
                {
                    throw new Exception($"Пользователь с Telegram ID {telegramId} уже существует.");
                }
                // Создаем новый объект User
                var user = new User
                {
                    TelegramId = telegramId,
                    PrefersTelegram = prefersTelegram,
                    Login = "tg_user", // По умолчанию
                    Password = "tg_user", // По умолчанию
                    CrmId = 1, // По умолчанию
                    CrmLogin = "tg_user", // По умолчанию
                    CrmPassword = "tg_user", // По умолчанию
                    Proxy = "", // По умолчанию
                    SyncFreq = "10", // Задайте значение по умолчанию
                    SyncSwitch = "ON", // Задайте значение по умолчанию
                    UpdateStatus = null, // По умолчанию
                    UpdateTime = null, // По умолчанию
                    Code = 0, // Задайте значение по умолчанию
                    PrefersCrm = "0", // Задайте значение по умолчанию
                    PrefersWhatsapp = "0", // Задайте значение по умолчанию
                    AllHistory = "", // По умолчанию
                    TelegramState = null, // По умолчанию
                    ChatId = null, // По умолчанию
                    DiagramProbability = 50, // Задайте значение по умолчанию
                    ManualProbability = 25 // Задайте значение по умолчанию
                };

                // Добавляем объект в контекст
                await _dbContext.Users.AddAsync(user);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при сохранении пользователя: {ex}");
                throw;
            }
        }

        public async Task SaveUserToBDAsync(User user)
        {
            try
            {
                // Проверяем, что объект user не равен null
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Пользователь не может быть null");
                }

                // Добавляем объект в контекст
                await _dbContext.Users.AddAsync(user);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при сохранении пользователя: {ex}");
            }
        }

        public async Task UpdateUserInBDAsync(User user)
        {
            try
            {
                // Проверяем, что объект user не равен null
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Пользователь не может быть null");
                }

                // Находим существующего пользователя по Id
                var existingUser = await _dbContext.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    throw new Exception($"Пользователь с Id {user.Id} не найден");
                }

                // Обновляем поля существующего пользователя
                existingUser.Login = user.Login;
                existingUser.Password = user.Password;
                existingUser.CrmId = user.CrmId;
                existingUser.CrmLogin = user.CrmLogin;
                existingUser.CrmPassword = user.CrmPassword;
                existingUser.Proxy = user.Proxy;
                existingUser.SyncFreq = user.SyncFreq;
                existingUser.SyncSwitch = user.SyncSwitch;
                existingUser.UpdateStatus = user.UpdateStatus;
                existingUser.UpdateTime = user.UpdateTime;
                existingUser.Code = user.Code;
                existingUser.PrefersCrm = user.PrefersCrm;
                existingUser.PrefersWhatsapp = user.PrefersWhatsapp;
                existingUser.PrefersTelegram = user.PrefersTelegram;
                existingUser.AllHistory = user.AllHistory;
                existingUser.TelegramId = user.TelegramId;
                existingUser.TelegramState = user.TelegramState;
                existingUser.ChatId = user.ChatId;
                existingUser.DiagramProbability = user.DiagramProbability;
                existingUser.ManualProbability = user.ManualProbability;

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при обновлении пользователя: {ex}");
            }
        }

        #endregion
    }
}
