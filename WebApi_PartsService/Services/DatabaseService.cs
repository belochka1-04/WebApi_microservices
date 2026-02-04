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
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Services
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

        #region PartsAndReplaces
        public async Task<List<PartsAndReplace>> GetPartsAndReplacesAsync(List<int> replaceIds)
        {
            if (replaceIds == null || replaceIds.Count == 0)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Info("Список идентификаторов замен пуст или равен null.");
                return new List<PartsAndReplace>(); // Возвращаем пустой список, если входной список пуст
            }

            try
            {
                // Используем LINQ для получения деталей замен по идентификаторам
                var partsAndReplaces = await _dbContext.PartsAndReplaces
                    .Where(p => replaceIds.Contains(p.Id))
                    .ToListAsync();

                return partsAndReplaces;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении деталей замен для идентификаторов {string.Join(", ", replaceIds)}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении деталей замен для идентификаторов {string.Join(", ", replaceIds)}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
        }

        public async Task<PartsAndReplace?> Get(int id)
        {
            try
            {
                // UserStocks удален - он в другом микросервисе и его надо вызывать отдельно
                return await _dbContext
                    .PartsAndReplaces
                    .FirstOrDefaultAsync(part => part.Id == id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при получении детали по ID {id}: {ex.Message}");
                return null;
            }
        }

        // Новый метод: Получение последней созданной детали (после вставки)
        public async Task<PartsAndReplace?> GetLastInsertedByPartNumberAsync(string partNumber)
        {
            try
            {
                return await _dbContext.PartsAndReplaces
                    .Where(p => p.MainPartNumber == partNumber)
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения последней детали по номеру {partNumber}: {ex.Message}");
                return null;
            }
        }

        // Для POST /api/partsandreplaces - создание с возвратом объекта
        public async Task<PartsAndReplace> CreatePartAsync(string partNumber)
        {
            try
            {
                var entity = new PartsAndReplace
                {
                    MainPartNumber = partNumber,
                    Status = "0", // начальный статус (в процессе)
                    DateUpdate = DateTime.Now
                };

                await _dbContext.PartsAndReplaces.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                _logger.Info($"Создана деталь: ID={entity.Id}, Number={partNumber}");

                return entity; // EF уже заполнил Id после SaveChanges
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка создания детали {partNumber}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<PartsAndReplace>> GetPartsAndReplacesWithStateAsync(string state)
        {
            try
            {
                // Используем LINQ для получения деталей замен по идентификаторам
                var partsAndReplaces = await _dbContext.PartsAndReplaces
                    .Where(p => p.Status == state)
                    .ToListAsync();

                return partsAndReplaces;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении деталей со статусом {state}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении деталей зсо статусом {state}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
        }

        public async Task<List<PartsAndReplace>> GetPartsAndReplacesByUserIDAsync(int userID)
        {
            try
            {
                // Используем LINQ для получения деталей замен по идентификаторам
                var partsAndReplaces = await _dbContext.PartsAndReplaces
                   .Where(pr => (pr.Status == "3" || pr.Status == "2") &&
                                _dbContext.UserStocks.Any(us => us.PartsAndReplacesId == pr.Id && us.UserId == userID))
                   .ToListAsync();

                return partsAndReplaces;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении деталей для пользователя {userID}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении деталей для пользователя {userID}: {ex.Message}");
                return new List<PartsAndReplace>(); // Возвращаем пустой список в случае ошибки
            }
        }
        public async Task InsertPartsAndReplacesAsync(string partNumber)
        {
            try
            {
                var partsAndReplace = new PartsAndReplace
                {
                    MainPartNumber = partNumber
                };

                await _dbContext.PartsAndReplaces.AddAsync(partsAndReplace);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вставке реплейса (partNumber: {partNumber}): {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при вставке реплейса (partNumber: {partNumber}): {ex.Message}");
                throw;
            }

        }
        public async Task UpdatePartsAndReplacesStatusAsync(int Id, int status)//UpdateModelNumbersAsync in stocks
        {
            try
            {
                var result = await _dbContext.PartsAndReplaces.FindAsync(Id);
                if (result != null && result.Id > 0)
                {
                    result.DateUpdate = DateTime.Now;
                    result.Status = status.ToString();
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при обновлении статуса : {ex.Message}");
            }
        }

        public async Task UpdatePartsAndReplacesAsync(PartsAndReplace item)//UpdateModelNumbersAsync in stocks
        {
            try
            {
                if (item != null && item.Id > 0)
                {
                    item.DateUpdate = DateTime.Now;
                    var result = _dbContext.PartsAndReplaces.Update(item);
                    await _dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при обновлении PartsAndReplace : {ex.Message}");
            }
        }
        #endregion

        #region PartsNamesArchive
        public async Task<PartsNamesArchive> GetPartsNamesArchiveById(int id)
        {
            try
            {
                return await _dbContext.PartsNamesArchives.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении архивного названия детали: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return null; // Или можно вернуть новый объект PartsNamesArchive, если это необходимо
            }
        }

        public async Task<bool> DeletePartsNamesArchive(int id)
        {
            try
            {
                var partsNamesArchive = await _dbContext.PartsNamesArchives.FindAsync(id);
                if (partsNamesArchive == null)
                {
                    return false; // Запись не найдена
                }

                _dbContext.PartsNamesArchives.Remove(partsNamesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно удалено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении архивного названия детали: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при удалении
            }
        }

        public async Task<bool> UpdatePartsNamesArchive(PartsNamesArchive partsNamesArchive)
        {
            try
            {
                _dbContext.PartsNamesArchives.Update(partsNamesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно обновлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении архивного названия детали: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при обновлении
            }
        }

        public async Task<bool> AddPartsNamesArchive(PartsNamesArchive partsNamesArchive)
        {
            try
            {
                await _dbContext.PartsNamesArchives.AddAsync(partsNamesArchive);
                await _dbContext.SaveChangesAsync();
                return true; // Успешно добавлено
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении архивного названия детали: {ex.Message}");
                _logger.Error("Ошибка: " + ex);
                return false; // Ошибка при добавлении
            }
        }

        #endregion

        #region parts
        public async Task<List<Parts>> GetPartsByModelIdAsync(int modelId)
        {
            try
            {
                var a = await _dbContext.ModelParts.Include(x => x.Part)
                    .Where(p => p.ModelId == modelId).ToListAsync();
                // Используем LINQ для получения частей по модели
                if (a.Count > 0)
                {
                    var parts = await _dbContext.ModelParts.Include(x => x.Part)
                        .Where(p => p.ModelId == modelId).Select(x => x.Part)
                        .ToListAsync();
                    if (parts != null && parts.Count > 0)
                    {
                        for (int i = 0; i < parts.Count; i++)
                        {
                            parts[i].IdModel = modelId;
                        }
                    }
                    return parts;
                }
                else
                    return new List<Parts>();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении частей по модели (modelId: {modelId}): {ex.Message}");
                throw; // Пробрасываем исключение дальше, если нужно
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении частей по модели (modelId: {modelId}): {ex.Message}");
                throw;
            }
        }
        #endregion

        #region PartsRequests
        public async Task<PartsRequest> CreateAsync(string recognizedPartNumber, int userId)
        {
            try
            {
                var request = new PartsRequest
                {
                    UserId = userId,
                    RecognizedPartNumber = recognizedPartNumber,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PartsAndReplacesId = null // Изначально не привязано к детали
                };

                await _dbContext.PartsRequests.AddAsync(request);
                await _dbContext.SaveChangesAsync();

                _logger.Info($"Создан запрос на поиск детали: ID={request.Id}, Number={recognizedPartNumber}, User={userId}");

                return request;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка создания запроса на деталь {recognizedPartNumber}: {ex.Message}");
                throw;
            }
        }

        public async Task<PartsRequest?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbContext.PartsRequests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения запроса {id}: {ex.Message}");
                return null;
            }
        }

        #endregion

    }
}
