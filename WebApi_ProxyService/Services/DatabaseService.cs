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
using WebApi_ProxyService.Services;

namespace WebApi_ProxyService.Services
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

        #region Proxy
        public async Task<List<Proxy>> GetProxyAsync()
        {
            try
            {
                // Используем LINQ для получения деталей замен по идентификаторам
                var request = await _dbContext.Proxies
                    .ToListAsync();

                return request;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении прокси: {ex.Message}");
                return new List<Proxy>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении прокси: {ex.Message}");
                return new List<Proxy>(); // Возвращаем пустой список в случае ошибки
            }
        }

        public async Task UpdateProxyAsync(Proxy data)
        {
            try
            {
                // Проверяем, что объект user не равен null
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "Proxy не может быть null");
                }

                // Находим существующего пользователя по Id
                var existingData = await _dbContext.Proxies.FindAsync(data.Id);
                if (existingData == null)
                {
                    throw new Exception($"Proxy с Id {data.Id} не найден");
                }

                // Обновляем поля существующего пользователя
                existingData.IP = data.IP;
                existingData.Port = data.Port;
                existingData.IsActive = data.IsActive;
                existingData.Type = data.Type;


                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при обновлении Proxy: {ex}");
            }
        }

        #endregion
    }
}
