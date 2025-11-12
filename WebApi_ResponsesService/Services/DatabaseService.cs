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
using WebApi_ResponsesService.Services;

namespace WebApi_ResponsesService.Services
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

        #region Responses
        public async Task InsertResponceAsync(KameraData.Data.Models.Response responce)
        {

            if (responce != null)
            {
                try
                {
                    _dbContext.Responses.Add(responce);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Error("Ошибка:" + ex);
                }
            }

        }

        public async Task<List<KameraData.Data.Models.Response>> GetResponceByJobIdAsync(int jobId)
        {
            try
            {
                var responces = await _dbContext.Responses.ToListAsync();
                var resp = await _dbContext.Responses
                    .Where(j => j.JobId == jobId).ToListAsync();

                if (resp == null)
                {
                    throw new Exception("Responce not found.");
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении заданий: " + ex);
                return new List<KameraData.Data.Models.Response>(); // Возвращаем новый экземпляр Job в случае ошибки
                throw;
            }
        }
        #endregion
    }
}
