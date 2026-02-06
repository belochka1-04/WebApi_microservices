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
using WebApi_JobDocumentService.Services;

namespace WebApi_JobDocumentService.Services
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

        #region JobDocsModelInfo
        public async Task<List<JobDocsModelInfo>> GetJobDocModelInfos(int jobId)
        {
            try
            {
                return await _dbContext.JobDocsModelInfos.Where(x => x.JobId == jobId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб ид:" + jobId + " - " + ex);
                return new List<JobDocsModelInfo>();
            }
        }
        #endregion

        #region job_docs
        public async Task InsertJobDocsAsync(int jobId, int modelsId) // Изменено на modelsId
        {
            try
            {
                var jobDoc = new JobDoc
                {
                    JobId = jobId,
                    ModelsId = modelsId // Убедитесь, что используете ModelsId
                };

                await _dbContext.JobDocs.AddAsync(jobDoc);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вставке документа задания (jobId: {jobId}, modelsId: {modelsId}): {ex.Message}");
                // Можно добавить дополнительную обработку ошибок, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при вставке документа задания (jobId: {jobId}, modelsId: {modelsId}): {ex.Message}");
            }
        }

        public async Task InsertFullJobDocsAsync(JobDoc job) // Изменено на modelsId
        {
            try
            {
                await _dbContext.JobDocs.AddAsync(job);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вставке документа задания (jobId: {job.JobId}, modelsId: {job.ModelsId}): {ex.Message}");
                // Можно добавить дополнительную обработку ошибок, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при вставке документа задания (jobId: {job.JobId}, modelsId: {job.ModelsId}): {ex.Message}");
            }
        }

        public async Task InsertFullJobDocsBatchAsync(List<JobDoc> docs) // Изменено на modelsId
        {
            try
            {
                _dbContext.JobDocs.AddRange(docs);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при вставке документа задания (jobId: {docs.First().JobId}, modelsId: {docs.First().ModelsId}): {ex.Message}");
                // Можно добавить дополнительную обработку ошибок, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при вставке документа задания (jobId: {docs.First().JobId}, modelsId: {docs.First().ModelsId}): {ex.Message}");
            }
        }

        public async Task DeleteJobDocsAsync(int jobId, int modelId)
        {
            try
            {
                var jobDoc = await _dbContext.JobDocs
                    .FirstOrDefaultAsync(jd => jd.JobId == jobId && jd.ModelsId == modelId);

                if (jobDoc != null)
                {
                    _dbContext.JobDocs.Remove(jobDoc);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при удалении документа задания (jobId: {jobId}, modelId: {modelId}): {ex.Message}");
                // Дополнительная обработка ошибок может быть добавлена здесь, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при удалении документа задания (jobId: {jobId}, modelId: {modelId}): {ex.Message}");
            }
        }

        public async Task DeleteJobDocsByJobIdAsync(int jobId)
        {
            try
            {
                var jobDoc = _dbContext.JobDocs.Where(jd => jd.JobId == jobId);

                if (jobDoc != null)
                {
                    _dbContext.JobDocs.RemoveRange(jobDoc);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при удалении документа задания (jobId: {jobId}): {ex.Message}");
                // Дополнительная обработка ошибок может быть добавлена здесь, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при удалении документа задания (jobId: {jobId}): {ex.Message}");
            }
        }

        public async Task<JobDoc> GetMainDoc(int jobID)
        {
            try
            {
                if (_dbContext.JobDocs.Any(j => j.JobId == jobID && (j.partCountState == 1)))// убираем обратно
                {
                    return await _dbContext.JobDocs
                     .Where(j => j.JobId == jobID && (j.partCountState == 1))
                     .OrderByDescending(j => j.Part_counter).OrderByDescending(k => k.Document_Id)
                     .FirstOrDefaultAsync();
                }
                else
                    return new JobDoc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб ид:" + jobID + " - " + ex);
                return new JobDoc();
            }
        }

        public async Task<JobDoc> GetPdfDoc(int jobID)
        {
            try
            {
                if (_dbContext.JobDocs.Any(j => j.JobId == jobID && j.docState == 1 && j.DocumentType == "Diagram PDF"))
                {
                    return await _dbContext.JobDocs
                     .Where(j => j.JobId == jobID && j.docState == 1 && j.DocumentType == "Diagram PDF")
                     .OrderByDescending(j => j.Document_Id)
                     .FirstOrDefaultAsync();
                }
                else
                    return new JobDoc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб ид:" + jobID + " - " + ex);
                return new JobDoc();
            }
        }
        /// <summary>
        /// Получить ВСЕ JobDocs по jobId
        /// </summary>
        public async Task<List<JobDoc>> GetAllJobDocsByJobIdAsync(int jobId)
        {
            try
            {
                return await _dbContext.JobDocs
                    .Where(j => j.JobId == jobId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения JobDocs по jobId {jobId}: {ex.Message}");
                return new List<JobDoc>();
            }
        }

        /// <summary>
        /// Получить JobDocs по jobId с фильтрами
        /// </summary>
        public async Task<List<JobDoc>> GetJobDocsByJobIdWithFiltersAsync(
            int jobId,
            int? siteState = null,
            int? docState = null,
            int? partCountState = null,
            string? documentType = null,
            string? cleanedModel = null)
        {
            try
            {
                var query = _dbContext.JobDocs.Where(j => j.JobId == jobId);

                if (siteState.HasValue)
                    query = query.Where(j => j.siteState == siteState.Value);

                if (docState.HasValue)
                    query = query.Where(j => j.docState == docState.Value);

                if (partCountState.HasValue)
                    query = query.Where(j => j.partCountState == partCountState.Value);

                if (!string.IsNullOrEmpty(documentType))
                    query = query.Where(j => j.DocumentType == documentType);

                if (!string.IsNullOrEmpty(cleanedModel))
                    query = query.Where(j => j.CleanedModel == cleanedModel);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения JobDocs с фильтрами для jobId {jobId}: {ex.Message}");
                return new List<JobDoc>();
            }
        }

        /// <summary>
        /// Получить JobDocs по списку ID
        /// </summary>
        public async Task<List<JobDoc>> GetJobDocsByIdsAsync(int[] ids)
        {
            try
            {
                return await _dbContext.JobDocs
                    .Where(j => ids.Contains(j.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка получения JobDocs по списку ID: {ex.Message}");
                return new List<JobDoc>();
            }
        }
    


        #endregion

#region job_docs_info
public async Task InsertJobDocsInfoAsync(int jobId, int taskId, int gotPartsListPdfId)
        {

            try
            {
                // Создаем новый объект JobDocInfo
                var jobDocInfo = new JobDocInfo
                {
                    JobId = jobId,
                    ModelsNumberId = taskId,
                    ModelId = gotPartsListPdfId
                };

                // Добавляем объект в контекст
                await _dbContext.JobDocsInfo.AddAsync(jobDocInfo);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при обновлении информации о документе для задачи с ID {jobId}: {ex.Message}");
                throw;
                // Здесь можно добавить дополнительную обработку ошибок, если необходимо
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при обновлении информации о документе для задачи с ID {jobId}: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
