using KameraData.Data;
using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_OnboardingVideoService.Services;

namespace WebApi_OnboardingVideoService.Services
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

		#region Onboarding Videos

		/// <summary>
		/// Получить следующее видео для ветки "repair" (модели)
		/// </summary>
		/// <param name="lastId">ID последнего просмотренного видео</param>
		public async Task<OnboardingVideo?> GetModelsAsync(int? lastId)
		{
			try
			{
				var video = await _dbContext
					.OnboardingVideos
					.Where(v => v.Branch == "repair")
					.OrderBy(v => v.Id)
					.Where(v => v.Id > (lastId ?? 0))
					.FirstOrDefaultAsync();

				if (video != null)
				{
					_logger.Info($"Получено видео моделей с ID: {video.Id}");
				}
				else
				{
					_logger.Info($"Видео моделей не найдено для lastId: {lastId}");
				}

				return video;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при получении видео моделей: {ex.Message}");
				return null;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Необработанная ошибка при получении видео моделей: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Получить следующее видео для ветки "warehouse" (запчасти)
		/// </summary>
		/// <param name="lastId">ID последнего просмотренного видео</param>
		public async Task<OnboardingVideo?> GetPartsAsync(int? lastId)
		{
			try
			{
				var video = await _dbContext
					.OnboardingVideos
					.Where(v => v.Branch == "warehouse")
					.OrderBy(v => v.Id)
					.Where(v => v.Id > (lastId ?? 0))
					.FirstOrDefaultAsync();

				if (video != null)
				{
					_logger.Info($"Получено видео запчастей с ID: {video.Id}");
				}
				else
				{
					_logger.Info($"Видео запчастей не найдено для lastId: {lastId}");
				}

				return video;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при получении видео запчастей: {ex.Message}");
				return null;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Необработанная ошибка при получении видео запчастей: {ex.Message}");
				return null;
			}
		}

		#endregion

		#region Issues

		public async Task<Issue?> GetIssueAsync(int issueId)
		{
			try
			{
				var issue = await _dbContext.Issues
					.FirstOrDefaultAsync(i => i.IssueId == issueId);

				if (issue != null)
				{
					_logger.Info($"Получена проблема с ID: {issueId}");
				}
				else
				{
					_logger.Warn($"Проблема с ID {issueId} не найдена");
				}

				return issue;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при получении проблемы с ID {issueId}");
				return null;
			}
		}

		public async Task<Issue> RegisterStockReportAsync(int userId, int stockId)
		{
			try
			{
				var issue = new Issue
				{
					Timestamp = DateTime.Now,
					Mode = "Part",
					UserId = userId,
					ScreenName = "3.2",
					IssueDetail = stockId.ToString(),
					IssueType = "20" // IssueType.Stock
				};

				await _dbContext.Issues.AddAsync(issue);
				await _dbContext.SaveChangesAsync();

				_logger.Info($"Зарегистрирован отчет по складу {stockId} от пользователя {userId}, IssueId: {issue.IssueId}");

				return issue;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при регистрации отчета по складу {stockId}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при регистрации отчета по складу {stockId}");
				throw;
			}
		}

		public async Task<Issue> RegisterPartReportAsync(int userId, int partId)
		{
			try
			{
				var issue = new Issue
				{
					Timestamp = DateTime.Now,
					Mode = "Part",
					UserId = userId,
					PartsAndReplacesId = partId,
					ScreenName = "2.3"
				};

				await _dbContext.Issues.AddAsync(issue);
				await _dbContext.SaveChangesAsync();

				_logger.Info($"Зарегистрирован отчет по запчасти {partId} от пользователя {userId}, IssueId: {issue.IssueId}");

				return issue;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при регистрации отчета по запчасти {partId}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при регистрации отчета по запчасти {partId}");
				throw;
			}
		}

		public async Task<Issue> RegisterSimilarReportAsync(int userId, int jobId)
		{
			try
			{
				var issue = new Issue
				{
					Timestamp = DateTime.Now,
					Mode = "Model",
					UserId = userId,
					JobId = jobId,
					ScreenName = "1.3.2"
				};

				await _dbContext.Issues.AddAsync(issue);
				await _dbContext.SaveChangesAsync();

				_logger.Info($"Зарегистрирован отчет по похожим моделям для job {jobId} от пользователя {userId}, IssueId: {issue.IssueId}");

				return issue;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при регистрации отчета по похожим моделям для job {jobId}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при регистрации отчета по похожим моделям для job {jobId}");
				throw;
			}
		}

		public async Task<Issue> RegisterModelReportAsync(int userId, int jobId, int modelId)
		{
			try
			{
				var issue = new Issue
				{
					Timestamp = DateTime.Now,
					Mode = "Model",
					UserId = userId,
					JobId = jobId,
					ScreenName = "1.3.3"
				};

				await _dbContext.Issues.AddAsync(issue);
				await _dbContext.SaveChangesAsync();

				_logger.Info($"Зарегистрирован отчет по модели {modelId} для job {jobId} от пользователя {userId}, IssueId: {issue.IssueId}");

				return issue;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при регистрации отчета по модели {modelId}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при регистрации отчета по модели {modelId}");
				throw;
			}
		}

		public async Task<Issue?> DetailIssueAsync(int issueId, int issueType, string? issueDetail)
		{
			try
			{
				var issue = await _dbContext.Issues.FindAsync(issueId);

				if (issue == null)
				{
					_logger.Warn($"Попытка обновить несуществующую проблему с ID {issueId}");
					return null;
				}

				issue.IssueType = issueType.ToString();
				issue.IssueDetail = issueDetail;

				await _dbContext.SaveChangesAsync();

				_logger.Info($"Обновлены детали проблемы {issueId}: тип={issueType}, детали={issueDetail}");

				return issue;
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, $"Ошибка БД при обновлении деталей проблемы {issueId}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при обновлении деталей проблемы {issueId}");
				throw;
			}
		}

		#endregion

		#region Tips

		/// <summary>
		/// Получить подсказки для пользователя
		/// </summary>
		public async Task<TipsResponse> GetTipsForUserAsync(int userId, int? lastVideoTipId, int? lastLinkTipId)
		{
			try
			{
				var video = await _dbContext.TipsVideos
					.Where(tip => tip.Id > (lastVideoTipId ?? 0))
					.OrderBy(tip => tip.Id)
					.FirstOrDefaultAsync();

				var link = await _dbContext.TipsLinks
					.Where(tip => tip.Id > (lastLinkTipId ?? 0))
					.OrderBy(tip => tip.Id)
					.FirstOrDefaultAsync();

				_logger.Info($"Получены подсказки для пользователя {userId}: VideoTip={video?.Id}, LinkTip={link?.Id}");

				return new TipsResponse
				{
					Video = video,
					Link = link
				};
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"Ошибка при получении подсказок для пользователя {userId}");
				throw;
			}
		}

		/// <summary>
		/// Зарегистрировать показ подсказок (уменьшить счетчики)
		/// </summary>
		public async Task RegisterTipsShowAsync()
		{
			try
			{
				var video = await _dbContext.TipsVideos
					.Where(tip => tip.RemainingShows > 0)
					.OrderBy(tip => tip.Id)
					.FirstOrDefaultAsync();

				var link = await _dbContext.TipsLinks
					.Where(tip => tip.RemainingShows > 0)
					.OrderBy(tip => tip.Id)
					.FirstOrDefaultAsync();

				if (video != null)
				{
					await _dbContext.TipsVideos
						.Where(tip => tip.Id == video.Id)
						.ExecuteUpdateAsync(setter =>
							setter.SetProperty(tip => tip.RemainingShows, video.RemainingShows - 1)
						);
					_logger.Info($"Уменьшен счетчик показов для TipsVideo ID={video.Id}, осталось: {video.RemainingShows - 1}");
				}

				if (link != null)
				{
					await _dbContext.TipsLinks
						.Where(tip => tip.Id == link.Id)
						.ExecuteUpdateAsync(setter =>
							setter.SetProperty(tip => tip.RemainingShows, link.RemainingShows - 1)
						);
					_logger.Info($"Уменьшен счетчик показов для TipsLink ID={link.Id}, осталось: {link.RemainingShows - 1}");
				}
			}
			catch (DbUpdateException ex)
			{
				_logger.Error(ex, "Ошибка БД при регистрации показа подсказок");
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка при регистрации показа подсказок");
				throw;
			}
		}

		#endregion
	}
}
