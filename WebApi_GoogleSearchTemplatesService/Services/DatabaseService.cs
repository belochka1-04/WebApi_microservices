using KameraData.Data;
using KameraData.Data.Dtos;
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
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Services
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

			try
			{
				await _dbContext.GoogleSerpRaws.AddRangeAsync(items);
				await _dbContext.SaveChangesAsync();

				var ids = items.Select(x => x.Id).ToList();
				_logger.Info("Inserted {Count} google_serp_raw records", ids.Count);
				return ids;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error inserting google_serp_raw batch");
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
