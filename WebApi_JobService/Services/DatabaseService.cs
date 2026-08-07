using KameraData.Data;
using KameraData.Data.Dtos;
using KameraData.Data.Models;
using KameraData.Events;
using MassTransit;
using MassTransit.Extensions.Hosting;
using MassTransit.Transports;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_JobService.Services;

namespace WebApi_JobService.Services
{
    public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
    {
        private readonly KameraDbContext _dbContext;
        private readonly NLog.ILogger _logger;
        private readonly UserServiceClient _userServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;  // ← ПОЛЕ (не локальная переменная)


        public DatabaseService(KameraDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<DatabaseService> logger)// ← DI автоматически                                                                               
        {
            _dbContext = dbContext;
            //_userServiceClient = userServiceClient;
            _publishEndpoint = publishEndpoint;  // ← ПРИСВАИВАЕМ ПОЛЕ
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<bool> IsDatabaseHealthyAsync()
        {
            try
            {
                // 🔥 +1: не только connect, но и query
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect) return false;

                // 🔥 +2: тестовая query
                var count = await _dbContext.JobDescriptionAndNotes
                    .FromSqlRaw("SELECT COUNT(*) FROM JobDescriptionAndNotes WITH (NOLOCK)")
                    .AsNoTracking()
                    .CountAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "База данных недоступна");
                return false;
            }
        }


        #region job_description_and_notes
        public async Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync()
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.Status == "0")
                    .AsNoTracking()  // 🔥 +1: read-only, быстрее
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении заданий"); // 🔥 +2: structured logging
                return Array.Empty<JobDescriptionAndNote>(); // 🔥 +3: вместо new List
            }
        }

        public async Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID)
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobID)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения JobDescriptionAndNote JobId={JobId}", jobID);
                return Array.Empty<JobDescriptionAndNote>();
            }
        }

        public async Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status)
        {
            try
            {
                if (jobId <= 0)
                {
                    _logger.Warn($"Некорректный JobId={jobId}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(j => j.Status, status.ToString()));

                    _logger.Info($"✅ Обновлено Status={status} для {updatedRows} записей JobId={jobId}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления Status JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                if (jobId <= 0 || string.IsNullOrWhiteSpace(fulltext))
                {
                    _logger.Warn($"Некорректные параметры: JobId={jobId}, fulltext length={fulltext?.Length ?? 0}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // Заменяем ' на пробел для SQL safety
                    var safeFulltext = fulltext.Replace('\'', ' ');

                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(j => j.JobDescription, safeFulltext));

                    _logger.Info($"✅ Обновлено JobDescription для {updatedRows} записей JobId={jobId}, length={safeFulltext.Length}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления JobDescription JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count)
        {
            try
            {
                if (jobId <= 0)
                {
                    _logger.Warn($"Некорректный JobId={jobId}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s.SetProperty(j => j.PicRecon, pic_count));

                    _logger.Info($"✅ Обновлено PicRecon={pic_count} для {updatedRows} записей JobId={jobId}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления PicRecon JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                if (jobId <= 0 || string.IsNullOrWhiteSpace(fulltext))
                {
                    _logger.Warn($"Некорректные параметры: JobId={jobId}, fulltext length={fulltext?.Length ?? 0}");
                    return;
                }

                var jobDescription = new JobDescriptionAndNote
                {
                    JobId = jobId,
                    JobDescription = fulltext.Replace('\'', ' '), // Escaping
                    Status = "0"
                };

                const int maxRetries = 3;
                var delayMs = 50; // Меньше для INSERT

                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        await _dbContext.JobDescriptionAndNotes.AddAsync(jobDescription);
                        await _dbContext.SaveChangesAsync();

                        _logger.Info($"✅ Добавлена JobDescriptionAndNote для JobId={jobId}, ID={jobDescription.Id}");
                        return;
                    }
                    catch (DbUpdateException ex) when (IsDeadlock(ex) && retry < maxRetries - 1)
                    {
                        _dbContext.Entry(jobDescription).State = EntityState.Detached; // Очистка change tracker
                        _logger.Warn($"⚠️ Deadlock INSERT #{retry + 1}/{maxRetries} для JobId={jobId}. Retry через {delayMs}ms");
                        await Task.Delay(delayMs);
                        delayMs *= 2; // 50, 100, 200ms
                    }
                }

                _logger.Error($"❌ INSERT failed после {maxRetries} попыток для JobId={jobId}");
                throw new InvalidOperationException($"Не удалось вставить JobDescriptionAndNote для JobId={jobId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Критическая ошибка INSERT JobDescriptionAndNote JobId={jobId}: {ex}");
                throw;
            }
        }



        public async Task MarkJobDescriptionsGoogleConfirmedAsync(string requestWord)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestWord)) return;
                var normalized = requestWord.Trim().ToUpper();

                const int maxRetries = 3;
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        var tableName = _dbContext.Model.FindEntityType(typeof(JobDescriptionAndNote))!.GetTableName()!;
                        var sql = $@"UPDATE [job_description_and_notes] WITH (ROWLOCK, UPDLOCK)
                            SET [google_confirmed] = 1           -- 🔥 snake_case!
                            WHERE ([google_confirmed] IS NULL OR [google_confirmed] = 0)
                            AND UPPER(LTRIM(RTRIM(ISNULL([job_description],'') + ISNULL([job_notes], '')))) = @p0";


                        var updatedRows = await _dbContext.Database
                            .ExecuteSqlRawAsync(sql, normalized);

                        _logger.Info($"✅ Raw SQL: обновлено {updatedRows} записей GoogleConfirmed для '{requestWord}'");
                        return;
                    }
                    catch (SqlException ex) when (IsTransientError(ex) && retry < maxRetries - 1)
                    {
                        _logger.Warn($"⚠️ SQL Error #{retry + 1}: {ex.Number} - {ex.Message}");
                        await Task.Delay(100 * (retry + 1));
                    }
                }
                throw new InvalidOperationException($"Failed after 3 retries for '{requestWord}'");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"💥 MarkJobDescriptionsGoogleConfirmedAsync failed: {requestWord}");
                throw;
            }
        }

        private static bool IsTransientError(SqlException ex) =>
            ex.Number is 1205 or 1222 or 49918 or 49919; // Deadlock, timeout, transient


        private static bool IsDeadlock(DbUpdateException ex) =>
         ex.InnerException is SqlException sqlEx &&
         (sqlEx.Number == 1205 /* deadlock */ || sqlEx.Number == 1222 /* lock timeout */);

        #endregion

        #region jobs
        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            try
            {
                var job = await _dbContext.Jobs.Include(x => x.User)
                    .Where(j => j.Id == jobId)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber,
                        JobLink = j.JobLink,
                        Token = j.Token,
                        UpdateRequestStatus = j.UpdateRequestStatus,
                        User = j.User,
                    })
                    .FirstOrDefaultAsync();

                if (job == null)
                {
                    throw new Exception("Job not found.");
                }

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении заданий: " + ex);
                return new Job(); // Возвращаем новый экземпляр Job в случае ошибки
            }
        }

        public async Task<JobDto?> GetJobDtoByIdAsync(int jobId)
        {
            try
            {
                var job = await _dbContext.Jobs
                    .Include(x => x.User)
                    .Where(j => j.Id == jobId)
                    .Select(j => new JobDto
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        TelegramId = j.TelegramId,
                        JobNumber = j.JobNumber,
                        JobLink = j.JobLink,
                        Token = j.Token,
                        UpdateRequestStatus = j.UpdateRequestStatus,
                        Status = j.Status
                    })
                    .FirstOrDefaultAsync();

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении Job");
                return null;
            }
        }
        public async Task<IEnumerable<Job>> GetJobByLink(string jobLink)
        {
            try
            {
                var jobs = await _dbContext.Jobs
                    .Where(j => j.JobLink == jobLink)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber
                    })
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб линк:" + jobLink + " - " + ex);
                return new List<Job>();
            }
        }

        
        public async Task<Job> AddJobByTgAsync(int telegramId)
        {
            var maxJobNumber = await _dbContext.Jobs
                .MaxAsync(j => Convert.ToInt32(j.JobNumber ?? "0"));  // ← Защита от null

            var newJob = new Job
            {
                TelegramId = telegramId,
                JobNumber = (maxJobNumber + 1).ToString(),
                Status = "Pending"
            };

            await _dbContext.Jobs.AddAsync(newJob);
            await _dbContext.SaveChangesAsync();
            
            // добавили _publishEndpoint
            await _publishEndpoint.Publish(new JobCreatedEvent
            {
                JobId = newJob.Id,
                TelegramId = telegramId
            });

            return newJob;
        }

        public async Task<int> GetTodaysOperationsCountAsync(int userId)
        {
            try
            {
                var today = DateTime.Today;

                return await _dbContext.Jobs
                    .Where(j => j.UserId == userId && j.CreatedAt >= today)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при подсчете операций за сегодня для пользователя {userId}", userId);
                return 0;
            }
        }

        public async Task<Job> CreateJobFromTextAsync(int userId, string? content, string? brand)
        {
            try
            {
                var job = new Job
                {
                    UserId = userId,
                    JobNumber = null,
                    JobLink = null,
                    Token = null,
                    UpdateRequestStatus = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Jobs.AddAsync(job);

                var note = new JobDescriptionAndNote
                {
                    Job = job,
                    JobId = job.Id,        // EF сам подставит после SaveChanges, но можно оставить явно
                    JobDescription = content,
                    OriginalBrand = brand,
                    Status = "0"
                };

                await _dbContext.JobDescriptionAndNotes.AddAsync(note);
                await _dbContext.SaveChangesAsync();

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при создании задания из текста для пользователя {userId}", userId);
                throw;
            }
        }

        public async Task<Job> CreateJobFromImageAsync(CreateJobFromImageDto dto)
        {
            try
            {
                var job = new Job
                {
                    UserId = dto.UserId,
                    JobNumber = null,
                    JobLink = null,
                    Token = null,
                    UpdateRequestStatus = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Jobs.AddAsync(job);

                var note = new JobDescriptionAndNote
                {
                    Job = job,
                    JobId = job.Id,
                    OriginalBrand = dto.Brand,
                    Model = dto.ModelNumber,
                    JobDescription = dto.ModelNumber,
                    StickerLink = dto.ImageUrl,
                    SerialNumber = dto.SerialNumber,
                    Status = "0"
                };

                await _dbContext.JobDescriptionAndNotes.AddAsync(note);

                // если позже решишь хранить распознавания, тут можно добавлять связанные сущности

                await _dbContext.SaveChangesAsync();

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при создании задания из изображения для пользователя {userId}", dto.UserId);
                throw;
            }
        }

        public async Task<Job?> GetFullJobByIdAsync(int jobId)
        {
            try
            {
                return await _dbContext.Jobs
                    .Include(j => j.JobDescriptionAndNote)
                    .Include(j => j.JobDocs)
                    .FirstOrDefaultAsync(j => j.Id == jobId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении полного Job {jobId}", jobId);
                return null;
            }
        }


        #endregion

    }
}
