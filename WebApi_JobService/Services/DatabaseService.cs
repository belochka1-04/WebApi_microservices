using JobService.Domain.Entities;
using JobService.Infrastructure.Persistence;
using Mapster;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using WebApi_JobService.Application.Dtos;
using WebApi_JobService.Application.Events;
using EFModels = WebApi_JobService.Infrastructure.Persistence.Entities;
using Domain = JobService.Domain.Entities;

namespace WebApi_JobService.Services;

public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
{
    private readonly IDbContextFactory<JobServiceDbContext> _dbFactory;
    private readonly NLog.ILogger _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public DatabaseService(
        IDbContextFactory<JobServiceDbContext> dbFactory,
        IPublishEndpoint publishEndpoint,
        ILogger<DatabaseService> logger)
    {
        _dbFactory = dbFactory;
        _publishEndpoint = publishEndpoint;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task<bool> IsDatabaseHealthyAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var canConnect = await db.Database.CanConnectAsync();
            if (!canConnect) return false;

            var count = await db.JobDescriptionAndNotes
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
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efList = await db.JobDescriptionAndNotes
                .Where(j => j.Status == "0")
                .AsNoTracking()
                .ToListAsync();

            return efList.Adapt<List<JobDescriptionAndNote>>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при получении заданий");
            return Array.Empty<JobDescriptionAndNote>();
        }
    }

    public async Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efList = await db.JobDescriptionAndNotes
                .Where(j => j.JobId == jobID)
                .AsNoTracking()
                .ToListAsync();

            return efList.Adapt<List<JobDescriptionAndNote>>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка получения JobDescriptionAndNote JobId={JobId}", jobID);
            return Array.Empty<JobDescriptionAndNote>();
        }
    }

    public async Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status)
    {
        if (jobId <= 0)
        {
            _logger.Warn($"Некорректный JobId={jobId}");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var updatedRows = await db.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ExecuteUpdateAsync(s => s.SetProperty(j => j.Status, status.ToString()));

                _logger.Info($"✅ Обновлено Status={status} для {updatedRows} записей JobId={jobId}");
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"💥 Ошибка обновления Status JobId={jobId}");
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

            await using var db = await _dbFactory.CreateDbContextAsync();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var safeFulltext = fulltext.Replace('\'', ' ');

                var updatedRows = await db.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(j => j.JobDescription, safeFulltext));

                _logger.Info($"✅ Обновлено JobDescription для {updatedRows} записей JobId={jobId}, length={safeFulltext.Length}");
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"💥 Ошибка обновления JobDescription JobId={jobId}");
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

            await using var db = await _dbFactory.CreateDbContextAsync();
            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var updatedRows = await db.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ExecuteUpdateAsync(s => s.SetProperty(j => j.PicRecon, pic_count));

                _logger.Info($"✅ Обновлено PicRecon={pic_count} для {updatedRows} записей JobId={jobId}");
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"💥 Ошибка обновления PicRecon JobId={jobId}");
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

            var jobDescription = new EFModels.JobDescriptionAndNote
            {
                JobId = jobId,
                JobDescription = fulltext.Replace('\'', ' '),
                Status = "0"
            };

            const int maxRetries = 3;
            var delayMs = 50;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                await using var db = await _dbFactory.CreateDbContextAsync();

                try
                {
                    await db.JobDescriptionAndNotes.AddAsync(jobDescription);
                    await db.SaveChangesAsync();

                    _logger.Info($"✅ Добавлена JobDescriptionAndNote для JobId={jobId}, ID={jobDescription.Id}");
                    return;
                }
                catch (DbUpdateException ex) when (IsDeadlock(ex) && retry < maxRetries - 1)
                {
                    db.Entry(jobDescription).State = EntityState.Detached;
                    _logger.Warn($"⚠️ Deadlock INSERT #{retry + 1}/{maxRetries} для JobId={jobId}. Retry через {delayMs}ms");
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
            }

            _logger.Error($"❌ INSERT failed после {maxRetries} попыток для JobId={jobId}");
            throw new InvalidOperationException($"Не удалось вставить JobDescriptionAndNote для JobId={jobId}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"💥 Критическая ошибка INSERT JobDescriptionAndNote JobId={jobId}");
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
                await using var db = await _dbFactory.CreateDbContextAsync();

                try
                {
                    var sql = @"UPDATE [job_description_and_notes] WITH (ROWLOCK, UPDLOCK)
                            SET [google_confirmed] = 1
                            WHERE ([google_confirmed] IS NULL OR [google_confirmed] = 0)
                            AND UPPER(LTRIM(RTRIM(ISNULL([job_description],'') + ISNULL([job_notes], '')))) = @p0";

                    var updatedRows = await db.Database.ExecuteSqlRawAsync(sql, normalized);

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
        ex.Number is 1205 or 1222 or 49918 or 49919;

    private static bool IsDeadlock(DbUpdateException ex) =>
        ex.InnerException is SqlException sqlEx &&
        (sqlEx.Number == 1205 || sqlEx.Number == 1222);

    #endregion

    #region jobs

    public async Task<Job> GetJobByIdAsync(int jobId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efJob = await db.Jobs
                .Include(x => x.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (efJob == null)
                throw new Exception("Job not found.");

            return efJob.Adapt<Job>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при получении Job {jobId}", jobId);
            return new Job();
        }
    }

    public async Task<JobDto?> GetJobDtoByIdAsync(int jobId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efJob = await db.Jobs
                .Include(x => x.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            return efJob?.Adapt<JobDto>();
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
            await using var db = await _dbFactory.CreateDbContextAsync();

            var jobs = await db.Jobs
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
            _logger.Error(ex, "Ошибка: джоб линк:{jobLink} - {ex}", jobLink, ex);
            return new List<Job>();
        }
    }

    public async Task<Job> AddJobByTgAsync(int telegramId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var maxJobNumber = await db.Jobs
            .MaxAsync(j => Convert.ToInt32(j.JobNumber ?? "0"));

        var efJob = new EFModels.Job
        {
            JobNumber = (maxJobNumber + 1).ToString()
        };

        await db.Jobs.AddAsync(efJob);
        await db.SaveChangesAsync();

        await _publishEndpoint.Publish(new JobCreatedEvent
        {
            JobId = efJob.Id,
            TelegramId = telegramId
        });

        return efJob.Adapt<Job>();
    }

    public async Task<int> GetTodaysOperationsCountAsync(int userId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var today = DateTime.Today;

            return await db.Jobs
                .Where(j => j.UserId == userId && j.CreatedAt >= today)
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при подсчете операций за сегодня для пользователя {userId}", userId);
            return 0;
        }
    }

    public async Task<Domain.Job> CreateJobFromTextAsync(int userId, string? content, string? brand)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efJob = new EFModels.Job
            {
                UserId = userId,
                JobNumber = null,
                JobLink = null,
                Token = null,
                UpdateRequestStatus = null,
                CreatedAt = DateTime.UtcNow
            };

            await db.Jobs.AddAsync(efJob);

            var efNote = new EFModels.JobDescriptionAndNote
            {
                Job = efJob,
                JobId = efJob.Id,
                JobDescription = content,
                OriginalBrand = brand,
                Status = "0"
            };

            await db.JobDescriptionAndNotes.AddAsync(efNote);
            await db.SaveChangesAsync();

            return efJob.Adapt<Domain.Job>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при создании задания из текста для пользователя {userId}", userId);
            throw;
        }
    }

    public async Task<Domain.Job> CreateJobFromImageAsync(CreateJobFromImageDto dto)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efJob = new EFModels.Job
            {
                UserId = dto.UserId,
                JobNumber = null,
                JobLink = null,
                Token = null,
                UpdateRequestStatus = null,
                CreatedAt = DateTime.UtcNow
            };

            await db.Jobs.AddAsync(efJob);

            var efNote = new EFModels.JobDescriptionAndNote
            {
                Job = efJob,
                JobId = efJob.Id,
                OriginalBrand = dto.Brand,
                Model = dto.ModelNumber,
                JobDescription = dto.ModelNumber,
                StickerLink = dto.ImageUrl,
                SerialNumber = dto.SerialNumber,
                Status = "0"
            };

            await db.JobDescriptionAndNotes.AddAsync(efNote);
            await db.SaveChangesAsync();

            return efJob.Adapt<Domain.Job>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при создании задания из изображения для пользователя {userId}", dto.UserId);
            throw;
        }
    }

    public async Task<Domain.Job?> GetFullJobByIdAsync(int jobId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var efJob = await db.Jobs
                .Include(j => j.JobDescriptionAndNote)
                .Include(j => j.JobDocs)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            return efJob?.Adapt<Domain.Job>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ошибка при получении полного Job {jobId}", jobId);
            return null;
        }
    }

    #endregion
}