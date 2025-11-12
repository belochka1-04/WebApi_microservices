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
using WebApi_PartSourceService.Services;

namespace WebApi_PartSourceService.Services
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

        #region PartSource
        public async Task<PartSource> GetPartSourceById(int id)
        {
            try
            {
                return await _dbContext.PartSources.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении источника: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект PartSource, если это необходимо
            }
        }

        public async Task<PartSource> GetPartSourceByUrl(string url)
        {
            try
            {
                return await _dbContext.PartSources.FirstOrDefaultAsync(x => x.Link == url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении источника: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект PartSource, если это необходимо
            }
        }
        public async Task<List<PartSource>> GetPartSource()
        {
            try
            {
                return await _dbContext.PartSources.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении перечня: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект PartSource, если это необходимо
            }
        }
        public async Task<bool> DeletePartSource(int id)
        {
            try
            {
                var partSource = await _dbContext.PartSources.FindAsync(id);
                if (partSource == null)
                {
                    return false; // Запись не найдена
                }

                _dbContext.PartSources.Remove(partSource);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно удалено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении источника: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при удалении
            }
        }

        public async Task<bool> UpdatePartSource(PartSource partSource)
        {
            try
            {
                _dbContext.PartSources.Update(partSource);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно обновлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении источника: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при обновлении
            }
        }

        public async Task<bool> AddPartSource(PartSource partSource)
        {
            try
            {
                await _dbContext.PartSources.AddAsync(partSource);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно добавлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении источника: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при добавлении
            }
        }

        #endregion
    }
}
