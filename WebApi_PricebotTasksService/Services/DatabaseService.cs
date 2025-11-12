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
using WebApi_PricebotTasksService.Services;

namespace WebApi_PricebotTasksService.Services
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

        #region  pricebot_tasks
        public async Task<PricebotTask> GetPricebotTaskByIdAsync(int id)
        {
            try
            {
                if (_dbContext.PricebotTasks.Any(j => j.Id == id))
                {
                    return await _dbContext.PricebotTasks.Where(x => x.Id == id).Where(x => x.Id == id).FirstOrDefaultAsync();
                }
                else
                {
                    return null; // Or throw an exception
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting pricebot task with ID: {Id} from database.", id);
                return null; // Or throw an exception
            }
        }

        public async Task<List<PricebotTask>> GetPricebotTaskByIdByZeroStateAsync()
        {
            try
            {
                if (_dbContext.PricebotTasks.Any(j => j.Status == "0"))
                {
                    return await _dbContext.PricebotTasks.Where(y => y.Status == "0").ToListAsync();
                }
                else
                {
                    return null; // Or throw an exception
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting pricebot task from database.");
                return null; // Or throw an exception
            }
        }

        public async Task CreatePricebotTaskAsync(PricebotTask task)
        {
            try
            {
                _dbContext.PricebotTasks.Add(task);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка добавления записи.");
                throw; // Re-throw the exception so the controller can handle it.  Important!
            }
        }

        public async Task UpdatePricebotTaskAsync(PricebotTask task)
        {
            try
            {
                _dbContext.Entry(task).State = EntityState.Modified; // Use explicit state modification
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка обновления записи..", task.Id);
                throw; // Re-throw the exception
            }
        }

        public async Task DeletePricebotTaskAsync(int id)
        {
            try
            {
                var task = await _dbContext.PricebotTasks.FindAsync(id);
                if (task != null)
                {
                    _dbContext.PricebotTasks.Remove(task);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка удаления записи с ID: {Id} .", id);
                throw; // Re-throw the exception
            }
        }

        public async Task<bool> PricebotTaskExistsAsync(int id)
        {
            try
            {
                return await _dbContext.PricebotTasks.AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка проверки наличия записи ID: {Id}, отсутствует в базе.", id);
                return false; // Or throw, depending on your error handling policy
            }
        }
        #endregion
    }
}
