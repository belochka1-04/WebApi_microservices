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

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при получении пользователя по Id {id}");
                throw;
            }
        }

        public async Task<User> GetUserByTgAsync(int Id)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == Id);
                return user;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении  пользователя (TgId: {Id}): {ex.Message}");
                throw; // Пробрасываем исключение дальше, если нужно
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении пользователя (TgId: {Id}): {ex.Message}");
                throw;
            }
        }

        public async Task<User> CreateOrGetUserAsync(long telegramId, int? referralUserId)
        {
            try
            {
                // 1. Пытаемся найти по TelegramId
                var existing = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

                if (existing != null)
                    return existing;

                // 2. Опционально найдём реферала по внутреннему Id
                User? referral = null;
                if (referralUserId.HasValue)
                {
                    referral = await _dbContext.Users
                        .FirstOrDefaultAsync(u => u.Id == referralUserId.Value);

                    // сам себя реферить нельзя
                    if (referral?.TelegramId == telegramId)
                        referral = null;
                }

                var now = DateTime.Now;

                // 3. Создаём нового пользователя с дефолтами
                //    Логика по мотивам ApplianceBot.UsersService.create:
                //    ProUntil = +1 месяц без реферала, +2 месяца с рефералом
                var proMonths = referral is null ? 1 : 2;

                var user = new User
                {
                    TelegramId = telegramId,
                    PrefersTelegram = "1",
                    SyncSwitch = "OFF",

                    // 0 = repair/models по умолчанию
                    TgState = 0,

                    // новые поля
                    ProUntil = now.AddMonths(proMonths),
                    LastUpdatedDateTime = now,

                    // остальное — как в SaveDefaultUserToBDAsync
                    Login = "tg_user",
                    Password = "tg_user",
                    CrmId = 1,
                    CrmLogin = "tg_user",
                    CrmPassword = "tg_user",
                    Proxy = "",
                    SyncFreq = "10",
                    UpdateStatus = null,
                    UpdateTime = null,
                    Code = 0,
                    PrefersCrm = "0",
                    PrefersWhatsapp = "0",
                    AllHistory = "",
                    TelegramState = null,
                    ChatId = null,
                    DiagramProbability = 50,
                    ManualProbability = 25
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                // 4. Обновляем ProUntil у реферала, если он есть
                if (referral is not null)
                {
                    referral.ProUntil = (referral.ProUntil ?? now).AddMonths(2);
                    referral.LastUpdatedDateTime = now;
                    await _dbContext.SaveChangesAsync();
                }

                return user;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при создании/получении пользователя по TelegramId {telegramId}");
                throw;
            }
        }

        public async Task<bool> UpdateUserModeAsync(int userId, byte mode)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                user.TgState = mode;
                user.LastUpdatedDateTime = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при смене режима пользователя {userId} на {mode}");
                throw;
            }
        }

       
        public async Task<bool> UpdateLastVideoTipAsync(int userId, int tipId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                if (user.LastVideoTipId >= tipId)
                {
                    // игнорируем, как и раньше
                    return true;
                }

                user.LastVideoTipId = tipId;
                user.LastUpdatedDateTime = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при обновлении LastVideoTipId для пользователя {userId}");
                throw;
            }
        }

        public async Task<bool> UpdateLastLinkTipAsync(int userId, int tipId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                if (user.LastLinkTipId >= tipId)
                    return true;

                user.LastLinkTipId = tipId;
                user.LastUpdatedDateTime = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при обновлении LastLinkTipId для пользователя {userId}");
                throw;
            }
        }

        public async Task<bool> UpdateLastRepairVideoAsync(int userId, int videoId)
{
    try
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;

        if (user.LastRepairVideoId >= videoId)
            return true;

        user.LastRepairVideoId = videoId;
        user.LastUpdatedDateTime = DateTime.Now;

        await _dbContext.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(ex, $"Ошибка при обновлении LastRepairVideoId для пользователя {userId}");
        throw;
    }
}

public async Task<bool> UpdateLastWarehouseVideoAsync(int userId, int videoId)
{
    try
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;

        if (user.LastWarehouseVideoId >= videoId)
            return true;

        user.LastWarehouseVideoId = videoId;
        user.LastUpdatedDateTime = DateTime.Now;

        await _dbContext.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(ex, $"Ошибка при обновлении LastWarehouseVideoId для пользователя {userId}");
        throw;
    }
}




        public async Task<bool> ExtendProAsync(int userId, int months)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                var now = DateTime.Now;
                // AddMonths корректно учитывает длину месяцев и високосные годы [web:192][web:166]
                user.ProUntil = (user.ProUntil ?? now).AddMonths(months);
                user.LastUpdatedDateTime = now;

                await _dbContext.SaveChangesAsync(); // EF сам сформирует UPDATE и выполнит в транзакции [web:193]
                return true;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, $"Ошибка при продлении Pro пользователю {userId} на {months} месяцев");
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
