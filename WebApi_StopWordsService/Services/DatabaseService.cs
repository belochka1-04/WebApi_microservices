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
using WebApi_StopWordsService.Services;

namespace WebApi_StopWordsService.Services
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

        #region StopWords
        public async Task<IEnumerable<StopWords>> GetStopWordsID(int userID)
        {
            try
            {
                // Используем LINQ для получения стоп-слов пользователя
                var stopWords = await _dbContext.StopWord
                    .Where(sw => sw.UserId == userID)
                    .ToListAsync();

                return stopWords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: не получилось найти стоп слова для пользователя: {userID} - {ex}");
                return Enumerable.Empty<StopWords>();
            }
        }
        #endregion
    }
}
