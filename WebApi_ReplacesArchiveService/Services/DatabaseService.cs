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
using WebApi_ReplacesArchiveService.Services;

namespace WebApi_ReplacesArchiveService.Services
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

        #region ReplacesArchive
        public async Task<ReplacesArchive> GetReplacesArchiveById(int id)
        {
            try
            {
                return await _dbContext.ReplacesArchives.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении реплейса: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект ReplacesArchive, если это необходимо
            }
        }

        public async Task<bool> DeleteReplacesArchive(int id)
        {
            try
            {
                var replacesArchive = await _dbContext.ReplacesArchives.FindAsync(id);
                if (replacesArchive == null)
                {
                    return false; // Запись не найдена
                }

                _dbContext.ReplacesArchives.Remove(replacesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно удалено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении реплейса: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при удалении
            }
        }

        public async Task<bool> UpdateReplacesArchive(ReplacesArchive replacesArchive)
        {
            try
            {
                _dbContext.ReplacesArchives.Update(replacesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно обновлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении реплейса: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при обновлении
            }
        }

        public async Task<bool> AddReplacesArchive(ReplacesArchive replacesArchive)
        {
            try
            {
                await _dbContext.ReplacesArchives.AddAsync(replacesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно добавлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении реплейса: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при добавлении
            }
        }


        #endregion

    }
}
