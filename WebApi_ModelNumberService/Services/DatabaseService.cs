using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_ModelNumberService.Services;

namespace WebApi_ModelNumberService.Services
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

        #region model_numbers
        public async Task<List<ModelNumber>> GetWasteTasksAsync()
        {
            try
            {
                var wasteTasks = await _dbContext.ModelNumbers
                    .Where(m => m.Confirmed == ((int)KameraData.Data.Other.Status.InWork).ToString() &&
                                m.GotForWorkAt < DateTime.UtcNow.AddSeconds(-120))
                    .ToListAsync();

                return wasteTasks;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при получении задач: {ex.Message}");
                return new List<ModelNumber>(); // Возвращаем пустой список в случае ошибки
            }
        }

        public async Task<ModelNumber> GetNextTaskAsync()
        {
            try
            {
                return await _dbContext.ModelNumbers
                    .Where(m => m.Confirmed == "1")
                    .OrderBy(m => m.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при получении следующей задачи: {ex.Message}");
                return null; // Возвращаем null в случае ошибки
            }
        }

        public async Task<List<ModelNumber>> GetNextTasksWithConfAsync(string Conf)
        {
            try
            {
                return await _dbContext.ModelNumbers
                    .Where(m => m.Confirmed == Conf).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при получении следующей задачи: {ex.Message}");
                return new List<ModelNumber>(); // Возвращаем null в случае ошибки
            }
        }
        public async Task<ModelNumber> GetNextTaskWithConfAsync(string Conf)
        {
            try
            {
                return await _dbContext.ModelNumbers
                    .Where(m => m.Confirmed == Conf)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при получении следующей задачи: {ex.Message}");
                return null; // Возвращаем null в случае ошибки
            }
        }
        public async Task InsertModelNumberAsync(int jobId, KameraData.Data.Models.Model model, string search_text, int status, int count)
        {
            try
            {
                var modelNumber = new ModelNumber
                {
                    JobId = jobId,
                    ModelNumber1 = model.model,
                    Brand = model.Brand,
                    MN_request = search_text,
                    Confirmed = status.ToString(),
                    DocCounter = count,
                    jdConfirmed = 0,
                    CleanedModel = model.CleanedModel
                };

                await _dbContext.ModelNumbers.AddAsync(modelNumber);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при вставке модели номера: {ex.Message}");
                throw;
            }
        }

        public async Task InsertModelNumberBatchAsync(List<ModelNumberRequest> requests)
        {
            int jobID = 0;
            if (requests == null || !requests.Any())
            {
                _logger.Warn("Получен пустой список запросов на пакетную вставку ModelNumber.");
                return; // Или можно вернуть BadRequest, если это ошибка
            }

            try
            {
                // Создаем список ModelNumber для пакетной вставки в базу данных
                var modelNumbers = requests.Select(request => new ModelNumber
                {
                    JobId = request.JobId,
                    ModelNumber1 = request.Model.model, // Access the Model property from the request
                    Brand = request.Model.Brand,       // Access the Brand property from the request
                    MN_request = request.SearchText,
                    Confirmed = request.Status.ToString(),
                    DocCounter = request.Count,
                    jdConfirmed = 0,
                    CleanedModel = request.Model.CleanedModel
                }).ToList();

                await _dbContext.ModelNumbers.AddRangeAsync(modelNumbers); // Добавляем весь список в контекст
                await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных

                var sql = "EXEC ProcessModelNumbersForJob @JobId";
                await _dbContext.Database.ExecuteSqlRawAsync(sql, new SqlParameter("@JobId", requests.First().JobId));

                _logger.Info($"Успешно вставлено {modelNumbers.Count} ModelNumber записей пакетно.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при пакетной вставке ModelNumber: {ex.Message}");
                throw; // Пробросить исключение, чтобы обработать его выше (например, в контроллере)
            }
        }

        public async Task InsertModelNumberBatchAsync(int jobId, KameraData.Data.Models.Model model, string search_text, int status, int count)
        {
            try
            {
                var modelNumber = new ModelNumber
                {
                    JobId = jobId,
                    ModelNumber1 = model.model,
                    Brand = model.Brand,
                    MN_request = search_text,
                    Confirmed = status.ToString(),
                    DocCounter = count,
                    jdConfirmed = 0
                };

                await _dbContext.ModelNumbers.AddAsync(modelNumber);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при вставке модели номера: {ex.Message}");
                throw;
            }
        }

        public async Task InsertNModelNumberAsync(int jobId, KameraData.Data.Models.NModel model, string search_text, int status, int count)
        {
            try
            {
                var modelNumber = new ModelNumber
                {
                    JobId = jobId,
                    ModelNumber1 = model.model,
                    Brand = model.Brand.Brand,
                    MN_request = search_text,
                    Confirmed = status.ToString(),
                    DocCounter = count
                };

                await _dbContext.ModelNumbers.AddAsync(modelNumber);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при вставке модели номера: {ex.Message}");
            }
        }
        public async Task DeleteModelNumbersAsync(int jobId)
        {
            try
            {
                var modelNumbers = await _dbContext.ModelNumbers
                    .Where(m => m.JobId == jobId)
                    .ToListAsync();

                _dbContext.ModelNumbers.RemoveRange(modelNumbers);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при удалении моделей номеров: {ex.Message}");
            }

        }
        public async Task UpdateOtherModelNumbersStatusAsync(int jobId, int status)
        {
            try
            {
                var modelNumbers = await _dbContext.ModelNumbers
                    .Where(m => m.JobId == jobId)
                    .ToListAsync();

                foreach (var modelNumber in modelNumbers)
                {
                    modelNumber.Confirmed = status.ToString();
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при обновлении статуса моделей номеров: {ex.Message}");
            }
        }
        public async Task UpdateTaskStatusAsync(int taskId, int status)//UpdateModelNumbersAsync in stocks
        {
            try
            {
                var modelNumber = await _dbContext.ModelNumbers.FindAsync(taskId);
                if (modelNumber != null)
                {
                    if (status == -1)
                        modelNumber.GotForWorkAt = DateTime.Now;
                    modelNumber.Confirmed = status.ToString();
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при обновлении статуса задачи с ID {taskId}: {ex.Message}");
            }
        }
        #endregion

        #region models_numbers_not_found
        public async Task InsertModelNumberNotFoundAsync(int jobId, string modelNumber)
        {
            try
            {
                if (!_dbContext.ModelsNumbersNotFounds.Any(z => z.JobId == jobId && z.ModelNumber == modelNumber))
                {
                    var modelNumberNotFound = new ModelsNumbersNotFound
                    {
                        JobId = jobId,
                        ModelNumber = modelNumber
                    };

                    await _dbContext.ModelsNumbersNotFounds.AddAsync(modelNumberNotFound);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при вставке модели номера не найденного: {ex.Message}");
            }

        }
        public async Task DeleteModelNumbersNotFoundAsync(int jobId)
        {
            try
            {
                var modelNumbersNotFound = await _dbContext.ModelsNumbersNotFounds
                    .Where(m => m.JobId == jobId)
                    .ToListAsync();

                _dbContext.ModelsNumbersNotFounds.RemoveRange(modelNumbersNotFound);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при удалении моделей номеров не найденных: {ex.Message}");
            }
        }
        #endregion
    }
}
