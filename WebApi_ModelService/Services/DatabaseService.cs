using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_ModelService.Services;

namespace WebApi_ModelService.Services
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
       
        #region models
        public async Task<List<KameraData.Data.Models.Model>> FindExactMatchInModelsAsync(string text)
        {
            var models = new List<KameraData.Data.Models.Model>();
            try
            {
                models = _dbContext.Models.Where(x => x.CleanedModel.ToUpper() == text.ToUpper().Trim()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: " + ex);
            }
            return models;
        }

        public async Task<List<KameraData.Data.Models.Model>> GetAllModelsAsync()
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var a = _dbContext.Models.Count();
                return await _dbContext.Models.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<Model>();
            }

        }

        public async Task<List<KameraData.Data.Models.NModel>> GetAllNModelsAsync()
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var a = _dbContext.NModels.Count();
                return await _dbContext.NModels.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<NModel>();
            }
        }

        public async Task<KameraData.Data.Models.Model> GetModelById(int Id)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                return await _dbContext.Models.FirstOrDefaultAsync(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new Model();
            }
        }

        public async Task<KameraData.Data.Models.NModel> GetNModelById(int Id)
        {
            try
            {
                return await _dbContext.NModels.FirstOrDefaultAsync(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new NModel();
            }
        }

        public async Task<KameraData.Data.Models.Model> GetModelById2(int Id)
        {
            var models = new KameraData.Data.Models.Model();

            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                return _dbContext.Models.FirstOrDefault(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: " + ex);
            }
            return models;
        }

        public async Task<KameraData.Data.Models.NModel> GetNModelById2(int Id)
        {
            var models = new KameraData.Data.Models.NModel();

            try
            {
                return _dbContext.NModels
                    .Include(n => n.Confidence) // Включаем связанную сущность Confidence
                    .Include(n => n.Brand) // Включаем связанную сущность Brand
                    .FirstOrDefault(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: " + ex);
            }
            return models;
        }

        public async Task<List<KameraData.Data.Models.Model>> FindSubstringMatchInModelsAsync(string modelNumber)
        {
            var models = new List<KameraData.Data.Models.Model>();

            try
            {
                _dbContext.Database.SetCommandTimeout(180);
                models = await _dbContext.Models
                   .FromSqlRaw("EXEC [dbo].[GetModelsLikeName] @modelName = {0}", modelNumber + "%".Trim().ToUpper())
                   .AsNoTracking()
                   .OrderByDescending(x => x.DateModel).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: " + ex);
            }
            return models;

        }

        public async Task<List<KameraData.Data.Models.NModel>> FindSubstringMatchInNModelsAsync(string modelNumber)
        {
            var models = new List<KameraData.Data.Models.NModel>();

            try
            {
                models = _dbContext.NModels
                    .Include(n => n.Confidence) // Включаем связанную сущность Confidence
                    .Include(n => n.Brand) // Включаем связанную сущность Brand
                    .Where(x => x.CleanedModel.ToUpper().StartsWith(modelNumber.Trim().ToUpper())).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: " + ex);
            }
            return models;

        }
        public async Task<List<KameraData.Data.Models.Model>> GetModelsWithNumAsync(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);
                //var ids = await _dbContext.NModels.Where(x => x.CleanedModel.ToUpper().StartsWith(modelNumber.ToUpper())).Select(x => x.Id).ToListAsync();

                //return await _dbContext.Models
                //    .Where(x => ids.Contains(x.Id))
                //    .OrderByDescending(x => x.DateModel)
                //    .ToListAsync();
                //var idList = string.Join(",", ids);

                // Выполняем хранимую процедуру и получаем результаты
                var models = await _dbContext.Models
                    .FromSqlRaw("EXEC [dbo].[GetModelsLikeName] @modelName = {0}", modelNumber + "%".Trim().ToUpper())
                    .AsNoTracking()
                    .ToListAsync();//.OrderByDescending(x => x.DateModel)

                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<Model>();
            }
        }

        public async Task<List<KameraData.Data.Models.Model>> GetModelsWithNum3Async(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);
                //var ids = await _dbContext.NModels.Where(x => x.CleanedModel.ToUpper().StartsWith(modelNumber.ToUpper())).Select(x => x.Id).ToListAsync();

                //return await _dbContext.Models
                //    .Where(x => ids.Contains(x.Id))
                //    .OrderByDescending(x => x.DateModel)
                //    .ToListAsync();
                //var idList = string.Join(",", ids);

                // Выполняем хранимую процедуру и получаем результаты
                var models = await _dbContext.Models
                    .FromSqlRaw("EXEC [dbo].[GetModelsLikeName3] @modelName = {0}", modelNumber + "%".Trim().ToUpper())
                    .AsNoTracking()
                    .ToListAsync();//.OrderByDescending(x => x.DateModel)

                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<Model>();
            }
        }

        public async Task<List<KameraData.Data.Models.JobDoc>> GetJDModelsWithNumAsync(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                // Выполняем хранимую процедуру и получаем результаты
                var jdmodels = await _dbContext.JobDocs
                    .FromSqlRaw("EXEC [dbo].[GetJobDocModelsLikeName] @modelName = {0}", modelNumber + "%".Trim().ToUpper())
                    .AsNoTracking()
                    .ToListAsync();

                return jdmodels;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<JobDoc>();
            }
        }

        public async Task GetJobDocModelsLikeNameAndInsertAsync(string modelNumber, int jobId, int maxLen, int minLen)
        {
            try
            {
                // Вызываем хранимую процедуру с помощью ExecuteSqlRawAsync
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[GetJobDocModelsLikeNameAndInsert2] @modelName = {0}, @jobId = {1}, @maxLen = {2}, @minLen = {3}",
                    modelNumber,
                    jobId,
                    maxLen,
                    minLen);

                // Дополнительная логика после успешного выполнения процедуры (если необходимо)
                Console.WriteLine($"Успешно вызвана хранимая процедура GetJobDocModelsLikeNameAndInsert с параметрами: modelName = {modelNumber}, jobId = {jobId}");
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при вызове хранимой процедуры GetJobDocModelsLikeNameAndInsert: {ex.Message}");
                // Логирование ошибки
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вызове хранимой процедуры GetJobDocModelsLikeNameAndInsert: {ex.Message}");
                // Возможно, стоит пробросить исключение дальше, чтобы обработать его на более высоком уровне
                throw;
            }
        }

        public async Task<List<string>> GetModelsLevaAsync(string InputTitle, string? InputBrandCode)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                // Вызываем хранимую процедуру с помощью ExecuteSqlRawAsync
                var models = await _dbContext.LevaModels.FromSqlRaw(
                    "EXEC [dbo].[CountModelDocumentsByTitleAndBrand3] @InputTitle = {0}, @InputBrandCode = {1}",
                    InputTitle,
                    InputBrandCode is null ? "Null" : InputBrandCode)
                    .ToListAsync();

                return (models.Any() && !string.IsNullOrEmpty(models.First().Title)) ? models.Select(x => x.Title).ToList() : new List<string>();
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при вызове хранимой процедуры CountModelDocumentsByTitleAndBrand3: {ex.Message}");
                // Логирование ошибки
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вызове хранимой процедуры CountModelDocumentsByTitleAndBrand3: {ex.Message}");
                // Возможно, стоит пробросить исключение дальше, чтобы обработать его на более высоком уровне
                throw;
            }
        }


        public async Task<List<KameraData.Data.Models.NModel>> GetNModelsWithNumAsync(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                return await _dbContext.NModels
                    .Include(n => n.Confidence) // Включаем связанную сущность Confidence
                    .Include(n => n.Brand) // Включаем связанную сущность Brand
                    .Where(x => x.CleanedModel.ToUpper().StartsWith(modelNumber.ToUpper()))
                    // .OrderByDescending(x => x.DateModel)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<NModel>();
            }
        }

        public async Task<List<KameraData.Data.Models.Model>> GetModelsWithNumForTrimAsync(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                // Выполняем хранимую процедуру и получаем результаты
                var models = await _dbContext.Models
                    .FromSqlRaw("EXEC [dbo].[GetModelsLikeName] @modelName = {0}", modelNumber.Trim().ToUpper())
                    .AsNoTracking()
                    .ToListAsync();

                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<Model>();
            }
        }

        public async Task<List<KameraData.Data.Models.Model>> GetModelsWithNumForTrim3Async(string modelNumber)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                // Выполняем хранимую процедуру и получаем результаты
                var models = await _dbContext.Models
                    .FromSqlRaw("EXEC [dbo].[GetModelsLikeName3] @modelName = {0}", modelNumber.Trim().ToUpper())
                    .AsNoTracking()
                    .ToListAsync();

                return models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<Model>();
            }
        }

        public async Task<List<KameraData.Data.Models.NModel>> GetNModelsWithNumForTrimAsync(string modelNumber)
        {
            try
            {
                return await _dbContext.NModels
                    .Include(n => n.Confidence) // Включаем связанную сущность Confidence
                    .Include(n => n.Brand) // Включаем связанную сущность Brand
                    .Where(x => x.CleanedModel.ToUpper() == modelNumber.ToUpper().Trim())
                    //  .OrderByDescending(x => x.DateModel)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return new List<NModel>();
            }
        }
        #endregion


        public async Task<Model?> GetModelByIdAsync(int id, bool includeBrandModel = false, bool includeSite = false)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var query = _dbContext.Models.AsQueryable();

                if (includeBrandModel)
                {
                    query = query.Include(m => m.BrandModel)
                                 .ThenInclude(bm => bm.Brand);
                }

                if (includeSite)
                {
                    query = query.Include(m => m.Site);
                }

                var model = await query.FirstOrDefaultAsync(m => m.Id == id);

                _logger.Info($"Получена модель {id}, Include: BrandModel={includeBrandModel}, Site={includeSite}");

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения модели {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Model>> GetModelsByIdsAsync(int[] ids, bool includeBrandModel = false, bool includeSite = false)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var query = _dbContext.Models.Where(m => ids.Contains(m.Id));

                if (includeBrandModel)
                {
                    query = query.Include(m => m.BrandModel)
                                 .ThenInclude(bm => bm.Brand);
                }
public async Task<Model?> GetModelByIdAsync(int id, bool includeBrandModel = false, bool includeSite = false)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var query = _dbContext.Models.AsQueryable();

                if (includeBrandModel)
                {
                    query = query.Include(m => m.BrandModel)
                                 .ThenInclude(bm => bm.Brand);
                }

                if (includeSite)
                {
                    query = query.Include(m => m.Site);
                }

                var model = await query.FirstOrDefaultAsync(m => m.Id == id);

                _logger.Info($"Получена модель {id}, Include: BrandModel={includeBrandModel}, Site={includeSite}");

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения модели {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Model>> GetModelsByIdsAsync(int[] ids, bool includeBrandModel = false, bool includeSite = false)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var query = _dbContext.Models.Where(m => ids.Contains(m.Id));

                if (includeBrandModel)
                {
                    query = query.Include(m => m.BrandModel)
                                 .ThenInclude(bm => bm.Brand);
                }

                if (includeSite)
                {
                    query = query.Include(m => m.Site);
                }

                var models = await query.ToListAsync();

                _logger.Info($"Получено {models.Count} моделей по списку ID (из {ids.Length} запрошенных)");

                return models;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения моделей по списку ID: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateModelLinkStateAsync(int id, int linkState)
        {
            try
            {
                _dbContext.Database.SetCommandTimeout(180);

                var model = await _dbContext.Models.FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    throw new KeyNotFoundException($"Модель с ID {id} не найдена");
                }

                var oldState = model.LinkState;
                model.LinkState = linkState;
                await _dbContext.SaveChangesAsync();

                _logger.Info($"Обновлено состояние ссылки модели {id}: {oldState} → {linkState}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка обновления состояния ссылки модели {id}: {ex.Message}");
                throw;
            }
        }
    }
