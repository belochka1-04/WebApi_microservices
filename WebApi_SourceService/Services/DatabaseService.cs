using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_SourceService.Services;

namespace WebApi_SourceService.Services
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

        #region sources
        public async Task<List<Sources>> GetAllSourcesAsync()
        {
            try
            {
                // Получаем все источники из базы данных с помощью Entity Framework
                var a = _dbContext.Sites.Count();
                return await _dbContext.Sites
                    .Select(j => new Sources
                    {
                        Id = j.Id,
                        Confidence = Convert.ToInt32(j.confidence),
                        DataTypes = j.DataTypes,
                        FolderPath = j.FolderPath,
                        SourceName = j.Title
                    })
                    .ToListAsync();

            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении всех источников: {ex.Message}");
                // Можно добавить дополнительную обработку ошибок, если необходимо
                return new List<Sources>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении всех источников: {ex.Message}");
                return new List<Sources>(); // Возвращаем пустой список в случае ошибки
            }
        }
        #endregion
    }
}
