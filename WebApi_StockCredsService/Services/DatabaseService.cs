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
using WebApi_StockCredsService.Services;

namespace WebApi_StockCredsService.Services
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

        #region stockCreds
        public async Task<List<StockCred>> GetStockCreds()
        {
            try
            {
                // Используем LINQ для получения запасов пользователя
                var result = await _dbContext.StockCreds.ToListAsync();

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

        public async Task UpdateStockCreds(StockCred stockCred)
        {
            try
            {
                _dbContext.StockCreds.Update(stockCred);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: + {ex}");
            }
        }


        #endregion
    }
}
