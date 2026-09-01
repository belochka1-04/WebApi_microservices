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
using WebApi_JobStockService.Services;

namespace WebApi_JobStockService.Services
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

        #region job_stocks
        public async Task<List<JobStocksView>> GetJobStocks(int jobId)
        {
            try
            {

                return await _dbContext.JobStocksViews.Where(x => x.JobId == jobId).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб ид:" + jobId + " - " + ex);
                return new List<JobStocksView>();
            }
        }
        public async Task DeleteJobStocksAsync(int jobId)
        {
            try
            {
                // Получаем все записи JobStock с указанным jobId
                var jobStocks = await _dbContext.JobStocks
                    .Where(js => js.JobId == jobId)
                    .ToListAsync();

                // Удаляем найденные записи
                _dbContext.JobStocks.RemoveRange(jobStocks);

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при удалении JobStocks для JobId {jobId}: {ex.Message}");
            }

        }

        public async Task DeleteJobStocksByUserIdAsync(int jobId, int userStockId)
        {
            try
            {
                // Получаем все записи JobStock с указанным jobId
                var jobStocks = await _dbContext.JobStocks
                    .Where(js => js.JobId == jobId && js.UserStockId == userStockId)
                    .ToListAsync();

                // Удаляем найденные записи
                _dbContext.JobStocks.RemoveRange(jobStocks);

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при удалении JobStocks для JobId {jobId}: {ex.Message}");
            }

        }

        //кажется более не используется ,не нашла сущности stock_search
        //public async Task UpdateStockSearchAsync(int taskId, int status)
        //{
        //    try
        //    {

        //        using var con = new SqlConnection(_connectionString);
        //        await con.OpenAsync();

        //        await con.ExecuteAsync($"UPDATE stock_search SET status='{status}' WHERE id='{taskId}';");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger logger = LogManager.GetCurrentClassLogger();
        //        logger.Error($"Ошибка при обновлении статуса задачи с ID {taskId}: {ex.Message}");
        //        // Здесь можно добавить дополнительную обработку ошибок, если необходимо
        //    }

        //}
        //а если используется надо реализовать этот код, реализовав _dbContext.StockSearches
        //public async Task UpdateStockSearchStatusAsync(int taskId, int status)
        //{
        //    try
        //    {
        //        // Ищем запись по ID
        //        var stockSearch = await _dbContext.StockSearches
        //            .FirstOrDefaultAsync(s => s.Id == taskId);

        //        if (stockSearch != null)
        //        {
        //            stockSearch.Status = status; // Обновляем статус
        //            await _dbContext.SaveChangesAsync(); // Сохраняем изменения
        //        }
        //        else
        //        {
        //            _logger.Error($"Запись StockSearch с ID {taskId} не найдена.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"Ошибка при обновлении StockSearch с ID {taskId}: {ex.Message}");
        //        // Дополнительная обработка ошибок при необходимости
        //    }
        //}

        public async Task InsertJobStocksAsync(int jobId, int userStockId, string matchPartNumber)
        {
            try
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();

                await _dbContext.JobStocks
                    .Where(js => js.JobId == jobId && js.UserStockId == userStockId)
                    .ExecuteDeleteAsync();

                var jobStock = new JobStock
                {
                    JobId = jobId,
                    UserStockId = userStockId,
                    MatchPartNumber = matchPartNumber
                };

                await _dbContext.JobStocks.AddAsync(jobStock);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вставке запасов работы (jobId: {jobId}, userStockId: {userStockId}, matchPartNumber: {matchPartNumber}): {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при вставке запасов работы (jobId: {jobId}, userStockId: {userStockId}, matchPartNumber: {matchPartNumber}): {ex.Message}");
                throw;
            }
        }
        public async Task DeleteJobStocksAsync(int jobId, int userStockId)
        {
            try
            {
                var jobStock = await _dbContext.JobStocks
                    .FirstOrDefaultAsync(js => js.JobId == jobId && js.UserStockId == userStockId);

                if (jobStock != null)
                {
                    _dbContext.JobStocks.Remove(jobStock);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при удалении запасов работы (jobId: {jobId}, userStockId: {userStockId}): {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при удалении запасов работы (jobId: {jobId}, userStockId: {userStockId}): {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
