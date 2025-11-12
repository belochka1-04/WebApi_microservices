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
using WebApi_PartsPicArchiveService.Services;

namespace WebApi_PartsPicArchiveService.Services
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

        #region PartsPicArchive
        public async Task<PartsPicArchive> GetPartsPicArchiveById(int id)
        {
            try
            {
                return await _dbContext.PartsPicArchives.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении архивного изображения: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект PartsPicArchive, если это необходимо
            }
        }

        public async Task<bool> DeletePartsPicArchive(int id)
        {
            try
            {
                var partsPicArchive = await _dbContext.PartsPicArchives.FindAsync(id);
                if (partsPicArchive == null)
                {
                    return false; // Запись не найдена
                }

                _dbContext.PartsPicArchives.Remove(partsPicArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно удалено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении архивного изображения: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при удалении
            }
        }

        public async Task<bool> UpdatePartsPicArchive(PartsPicArchive partsPicArchive)
        {
            try
            {
                _dbContext.PartsPicArchives.Update(partsPicArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно обновлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении архивного изображения: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при обновлении
            }
        }

        public async Task<bool> AddPartsPicArchive(PartsPicArchive partsPicArchive)
        {
            try
            {
                await _dbContext.PartsPicArchives.AddAsync(partsPicArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно добавлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении архивного изображения: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при добавлении
            }
        }

        #endregion
    }
}
