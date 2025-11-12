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
using WebApi_ErrorLogService.Services;

namespace WebApi_ErrorLogService.Services
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

        #region ErrorLog
        public async Task<List<ErrorLog>> GetErrorLogAsync()
        {
            try
            {
                // Используем LINQ для получения деталей замен по идентификаторам
                var request = await _dbContext.ErrorLogs
                    .ToListAsync();

                return request;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении ErrorLog: {ex.Message}");
                return new List<ErrorLog>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении ErrorLog: {ex.Message}");
                return new List<ErrorLog>(); // Возвращаем пустой список в случае ошибки
            }
        }

        public async Task<ErrorLog> GetErrorLogByIdAsync(int id)
        {
            try
            {
                return await _dbContext.ErrorLogs.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Log the error
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Error getting ErrorLog by Id {id}: {ex.Message}");
                return null; // Or throw the exception, depending on your error handling strategy
            }
        }

        public async Task<ErrorLog> InsertErrorLogAsync(ErrorLog errorLog)
        {
            try
            {
                _dbContext.ErrorLogs.Add(errorLog);
                await _dbContext.SaveChangesAsync();
                return errorLog;  // Return the saved object, including the generated ID
            }
            catch (Exception ex)
            {
                // Log the error
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Error inserting ErrorLog: {ex.Message}");
                throw; // Re-throw the exception so the controller can handle it
            }
        }

        public async Task DeleteErrorLogAsync(int id)
        {
            try
            {
                var errorLog = await _dbContext.ErrorLogs.FindAsync(id);
                if (errorLog != null)
                {
                    _dbContext.ErrorLogs.Remove(errorLog);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Error deleting ErrorLog: {ex.Message}");
                throw; // Re-throw the exception so the controller can handle it
            }
        }

        public async Task UpdateErrorLogAsync(ErrorLog errorLog)
        {
            try
            {
                _dbContext.Entry(errorLog).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Error updating ErrorLog: {ex.Message}");
                throw; // Re-throw the exception so the controller can handle it
            }
        }


        #endregion
    }
}
