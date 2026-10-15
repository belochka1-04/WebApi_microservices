using KameraData.Data;
using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
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
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Services
{
	public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
	{
		private readonly KameraDbContext _dbContext;
		private readonly NLog.ILogger _logger;
        private readonly Microsoft.Extensions.Logging.ILogger<DatabaseService> _batchLogger;

		public DatabaseService(KameraDbContext dbContext, Microsoft.Extensions.Logging.ILogger<DatabaseService> batchLogger)
		{
			_dbContext = dbContext;
            _batchLogger = batchLogger;
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
		#region google_serp_raw

		public async Task<int> InsertGoogleSerpRawAsync(GoogleSerpRaw serp)
		{
			if (serp == null) throw new ArgumentNullException(nameof(serp));

			try
			{
				_dbContext.GoogleSerpRaws.Add(serp);
				await _dbContext.SaveChangesAsync();
				_logger.Info("Inserted google_serp_raw Id={Id}", serp.Id);
				return serp.Id;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error inserting google_serp_raw");
				throw;
			}
		}

        public async Task<IReadOnlyList<int>> InsertGoogleSerpRawBatchAsync(IEnumerable<GoogleSerpRaw> serps)
        {
            var items = serps?.ToList() ?? new List<GoogleSerpRaw>();
            if (items.Count == 0)
                return Array.Empty<int>();

            var requestIds = items.Select(x => x.GoogleModelRequestId).Distinct().ToList();
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var stage = "begin_transaction";
            using var scope = _batchLogger.BeginScope(new Dictionary<string, object>
            {
                ["SerpRequestIds"] = string.Join(",", requestIds.Take(20)),
                ["SerpCount"] = items.Count
            });
            _batchLogger.LogInformation(
                "SERP batch started: Count={Count}, RequestCount={RequestCount}, MaxQueryLength={MaxQueryLength}, MaxTitleLength={MaxTitleLength}, MaxSnippetLength={MaxSnippetLength}, MaxUrlLength={MaxUrlLength}, InvalidLengthRows={InvalidLengthRows}, RetryingStrategy={RetryingStrategy}",
                items.Count, requestIds.Count,
                items.Max(x => x.QueryText?.Length ?? 0), items.Max(x => x.Title?.Length ?? 0),
                items.Max(x => x.Snippet?.Length ?? 0), items.Max(x => x.Url?.Length ?? 0),
                items.Count(x => x.QueryText == null || x.Url == null || x.QueryText.Length > 500 ||
                    (x.Title?.Length ?? 0) > 1000 || (x.Snippet?.Length ?? 0) > 2000 || x.Url.Length > 2000),
                _dbContext.Database.CreateExecutionStrategy().RetriesOnFailure);
            try
            {
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    // Each retry needs fresh entities: SaveChanges may have assigned identity IDs
                    // before a transient commit failure. Replacement by request ID is idempotent.
                    var attemptItems = items.Select(x => new GoogleSerpRaw
                    {
                        Id = x.Id,
                        GoogleModelRequestId = x.GoogleModelRequestId,
                        QueryText = x.QueryText,
                        GooglePage = x.GooglePage,
                        Title = x.Title,
                        Snippet = x.Snippet,
                        Url = x.Url,
                        CreatedAt = x.CreatedAt
                    }).ToList();
                    try
                    {
                        stage = "begin_transaction";
                        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                        stage = "delete_previous_results";
                        var deleted = await _dbContext.GoogleSerpRaws
                            .Where(x => requestIds.Contains(x.GoogleModelRequestId))
                            .ExecuteDeleteAsync();
                        stage = "track_new_results";
                        await _dbContext.GoogleSerpRaws.AddRangeAsync(attemptItems);
                        stage = "save_changes";
                        await _dbContext.SaveChangesAsync();
                        stage = "commit_transaction";
                        await transaction.CommitAsync();
                        _batchLogger.LogInformation(
                            "SERP batch committed: Count={Count}, Replaced={Replaced}, ElapsedMs={ElapsedMs}",
                            attemptItems.Count, deleted, timer.ElapsedMilliseconds);
                        return (IReadOnlyList<int>)attemptItems.Select(x => x.Id).ToList();
                    }
                    finally
                    {
                        foreach (var item in attemptItems)
                            _dbContext.Entry(item).State = EntityState.Detached;
                    }
                });
            }
            catch (Exception ex)
            {
                var sql = ex.GetBaseException() as SqlException;
                _batchLogger.LogError(ex,
                    "SERP batch failed: Stage={Stage}, Count={Count}, ElapsedMs={ElapsedMs}, SqlNumber={SqlNumber}, SqlState={SqlState}, SqlProcedure={SqlProcedure}, SqlLine={SqlLine}",
                    stage, items.Count, timer.ElapsedMilliseconds, sql?.Number, sql?.State, sql?.Procedure, sql?.LineNumber);
                throw;
            }
        }

		#endregion

		#region site_templates

		public async Task<List<SiteTemplate>> GetActiveSiteTemplatesAsync()
		{
			try
			{
				return await _dbContext.SiteTemplates
					.Where(t => t.IsActive)
					.OrderBy(t => t.SiteName)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error fetching active site templates");
				throw;
			}
		}

        public async Task<List<SiteTemplate>> GetActiveSiteTemplatesWithTitleRulesAsync()
        {
            try
            {
                return await _dbContext.SiteTemplates
                    .AsNoTracking()
                    .Where(t => t.IsActive)
                    .Include(t => t.TitleRules.Where(r => r.IsActive))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching active site templates with title rules");
                throw;
            }
        }


        public Task<List<SiteTemplateTitleRule>> GetActiveTitleRulesAsync(int siteTemplateId)
            => _dbContext.Set<SiteTemplateTitleRule>()
                .AsNoTracking()
                .Where(r => r.SiteTemplateId == siteTemplateId && r.IsActive)
                .OrderBy(r => r.Priority)
                .ToListAsync();

        public async Task<List<SiteTemplate>> GetAllSiteTemplatesAsync()
		{
			try
			{
				return await _dbContext.SiteTemplates
					.OrderBy(t => t.SiteName)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error fetching all site templates");
				throw;
			}
		}

		public async Task<SiteTemplate?> GetSiteTemplateByIdAsync(int id)
		{
			try
			{
				return await _dbContext.SiteTemplates.FirstOrDefaultAsync(t => t.Id == id);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error fetching site template Id={Id}", id);
				throw;
			}
		}

		public async Task<int> CreateSiteTemplateAsync(SiteTemplate template)
		{
			try
			{
				template.CreatedAt = DateTime.UtcNow;
				_dbContext.SiteTemplates.Add(template);
				await _dbContext.SaveChangesAsync();
				_logger.Info("Created SiteTemplate Id={Id}", template.Id);
				return template.Id;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error creating site template");
				throw;
			}
		}

		public async Task UpdateSiteTemplateAsync(SiteTemplate template)
		{
			try
			{
				template.UpdatedAt = DateTime.UtcNow;
				_dbContext.SiteTemplates.Update(template);
				await _dbContext.SaveChangesAsync();
				_logger.Info("Updated SiteTemplate Id={Id}", template.Id);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error updating site template Id={Id}", template.Id);
				throw;
			}
		}

		#endregion

		#region GoogleModelRequests

		public async Task<List<GoogleModelRequest>> GetPendingGoogleRequestsAsync(int batchSize)
		{
			try
			{
				return await _dbContext.GoogleModelRequests
					.Where(r => r.Status == 0)
					.OrderBy(r => r.CreatedAt)
					.Take(batchSize)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error fetching pending GoogleModelRequests");
				throw;
			}
		}

		public async Task<GoogleModelRequest> CreateGoogleModelRequestAsync(string request)
		{
			try
			{
				var entity = new GoogleModelRequest
				{
					Request = request,
					CreatedAt = DateTime.UtcNow,
					Status = 0
				};

				_dbContext.GoogleModelRequests.Add(entity);
				await _dbContext.SaveChangesAsync();

				_logger.Info("Created GoogleModelRequest Id={Id}", entity.Id);
				return entity;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error creating GoogleModelRequest");
				throw;
			}
		}

		public async Task MarkGoogleModelRequestProcessedAsync(int id)
		{
			await UpdateGoogleModelRequestStatusAsync(id, 1);
		}

		public async Task UpdateGoogleModelRequestStatusAsync(int id, byte status)
		{
			try
			{
				var entity = await _dbContext.GoogleModelRequests.FirstOrDefaultAsync(r => r.Id == id);
				if (entity == null)
				{
					_logger.Warn("GoogleModelRequest Id={Id} not found", id);
					return;
				}

				entity.Status = status;
				entity.LastCheckedAt = DateTime.UtcNow;

				await _dbContext.SaveChangesAsync();

				_logger.Info(
					"Updated GoogleModelRequest Id={Id} status to {Status}", id, status);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error updating GoogleModelRequest Id={Id}", id);
				throw;
			}
		}

		#endregion
	}

}
