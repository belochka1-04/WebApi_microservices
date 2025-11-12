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
using WebApi_MaskService.Services;

namespace WebApi_MaskService.Services
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

        #region Masks
        public async Task<IEnumerable<Mask>> GetAllMasksAsync(int jobID)
        {
            try
            {
                // Получаем работу по ID
                var job = await GetJobByIdAsync(jobID);

                // Проверяем, существует ли работа и у нее есть UserId
                if (job == null || job.UserId == 0)
                {
                    return Enumerable.Empty<Mask>(); // Возвращаем пустой список, если работа не найдена или UserId отсутствует
                }

                // Получаем все маски для данного userId
                var masks = await _dbContext.Masks
                    .Where(m => m.UserId == job.UserId)
                    .ToListAsync();

                return masks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return Enumerable.Empty<Mask>(); // Возвращаем пустой список в случае ошибки
                throw;
            }
        }
        #endregion
    }
}
