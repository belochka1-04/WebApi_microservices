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
using WebApi_ModelCatalogService.Services;

namespace WebApi_ModelCatalogService.Services
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



		#region site
		// Секция #region Sites - ИСПРАВЛЕННЫЕ МЕТОДЫ

		public async Task<Site?> GetSiteByIdAsync(int id)
		{
			try
			{
				return await _dbContext.Sites.FirstOrDefaultAsync(s => s.Id == id);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения сайта {Id}", id);
				throw;
			}
		}

		public async Task<List<Site>> GetSitesByIdsAsync(int[] ids)
		{
			try
			{
				return await _dbContext.Sites
					.Where(s => ids.Contains(s.Id))
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения сайтов по списку ID");
				throw;
			}
		}

		public async Task<List<Site>> GetAllSitesAsync()
		{
			try
			{
				// Сортировка по Confidence (строка varchar(2))
				return await _dbContext.Sites
					.OrderBy(s => s.confidence)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения всех сайтов");
				throw;
			}
		}

		/// <summary>
		/// Получить сайты с определенным значением Confidence
		/// </summary>
		/// <param name="confidence">Значение Confidence (varchar(2), например: A, B, C)</param>
		public async Task<List<Site>> GetSitesByConfidenceAsync(string confidence)
		{
			try
			{
				return await _dbContext.Sites
					.Where(s => s.confidence == confidence)
					.OrderBy(s => s.Title)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(
					ex,
					"Ошибка получения сайтов с Confidence = {Confidence}",
					confidence);
				throw;
			}
		}

		/// <summary>
		/// Поиск сайтов по названию (поиск подстроки)
		/// </summary>
		public async Task<List<Site>> SearchSitesByTitleAsync(string query)
		{
			try
			{
				var lowerQuery = query.ToLower();

				return await _dbContext.Sites
					.Where(s => s.Title != null && s.Title.ToLower().Contains(lowerQuery))
					.OrderBy(s => s.confidence)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка поиска сайтов по запросу: {Query}", query);
				throw;
			}
		}
		#endregion


		#region Brands

		public async Task<Brand?> GetBrandByIdAsync(int id)
		{
			try
			{
				return await _dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения бренда {Id}", id);
				throw;
			}
		}

		public async Task<List<Brand>> GetBrandsByIdsAsync(int[] ids)
		{
			try
			{
				return await _dbContext.Brands
					.Where(b => ids.Contains(b.Id))
					.OrderBy(b => b.Title)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения брендов по списку ID");
				throw;
			}
		}

		public async Task<List<Brand>> GetAllBrandsAsync()
		{
			try
			{
				return await _dbContext.Brands
					.Where(b => !string.IsNullOrWhiteSpace(b.Title))
					.OrderBy(b => b.Title)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения всех брендов");
				throw;
			}
		}

		/// <summary>
		/// Поиск брендов по названию (частичное совпадение, case-insensitive)
		/// </summary>
		public async Task<List<Brand>> SearchBrandsByTitleAsync(string query)
		{
			try
			{
				var lowerQuery = query.ToLower();

				return await _dbContext.Brands
					.Where(b => b.Title != null && b.Title.ToLower().Contains(lowerQuery))
					.OrderBy(b => b.Title)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка поиска брендов по запросу: {Query}", query);
				throw;
			}
		}

		/// <summary>
		/// Получить бренд по точному названию (case-insensitive)
		/// </summary>
		public async Task<Brand?> GetBrandByTitleAsync(string title)
		{
			try
			{
				var lowerTitle = title.ToLower();

				return await _dbContext.Brands
					.FirstOrDefaultAsync(b => b.Title != null && b.Title.ToLower() == lowerTitle);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка поиска бренда по названию: {Title}", title);
				throw;
			}
		}

		/// <summary>
		/// Получить популярные бренды (топ N по количеству моделей)
		/// </summary>
		public async Task<List<BrandWithCountDto>> GetPopularBrandsAsync(int top = 10)
		{
			try
			{
				var result = await _dbContext.Brands
					.Where(b => !string.IsNullOrWhiteSpace(b.Title))
					.Select(b => new BrandWithCountDto
					{
						Id = b.Id,
						Title = b.Title,
						ModelsCount = _dbContext.BrandModels
							.Where(bm => bm.BrandId == b.Id)
							.SelectMany(bm => bm.Models)
							.Count()
					})
					.OrderByDescending(b => b.ModelsCount)
					.Take(top)
					.ToListAsync();

				_logger.Info("Получено {Count} популярных брендов", result.Count);

				return result;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения популярных брендов");
				throw;
			}
		}

		/// <summary>
		/// Получить количество моделей для бренда
		/// </summary>
		/// <returns>Количество моделей или -1 если бренд не найден</returns>
		public async Task<int> GetBrandModelsCountAsync(int brandId)
		{
			try
			{
				var brandExists = await _dbContext.Brands.AnyAsync(b => b.Id == brandId);

				if (!brandExists)
				{
					return -1; // Бренд не найден
				}

				var count = await _dbContext.BrandModels
					.Where(bm => bm.BrandId == brandId)
					.SelectMany(bm => bm.Models)
					.CountAsync();

				return count;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения количества моделей для бренда {BrandId}", brandId);
				throw;
			}
		}

		#endregion

		#region Models

		public async Task<ModelTb?> GetModelByIdAsync(
			int id,
			bool includeBrandModel = false,
			bool includeSite = false)
		{
			try
			{
				var query = _dbContext.ModelTbs.AsQueryable();  // Models = DbSet<ModelTb>

				if (includeBrandModel)
				{
					query = query.Include(m => m.BrandModel!)
								 .ThenInclude(bm => bm.Brand);
				}

				if (includeSite)
				{
					query = query.Include(m => m.Site);
				}

				var model = await query.FirstOrDefaultAsync(m => m.Id == id);

				_logger.Debug(
					"Получена модель {Id}, Include: BrandModel={IncludeBrand}, Site={IncludeSite}",
					id, includeBrandModel, includeSite);

				return model;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения модели {Id}", id);
				throw;
			}
		}

		public async Task<List<ModelTb>> GetModelsByIdsAsync(
			int[] ids,
			bool includeBrandModel = false,
			bool includeSite = false)
		{
			try
			{
				var query = _dbContext.ModelTbs.Where(m => ids.Contains(m.Id));

				if (includeBrandModel)
				{
					query = query.Include(m => m.BrandModel!)
								 .ThenInclude(bm => bm.Brand);
				}

				if (includeSite)
				{
					query = query.Include(m => m.Site);
				}

				var models = await query.ToListAsync();

				_logger.Debug(
					"Получено {Count} моделей по списку ID (из {Total} запрошенных)",
					models.Count, ids.Length);

				return models;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка получения моделей по списку ID");
				throw;
			}
		}

		public async Task UpdateModelLinkStateAsync(int id, int linkState)
		{
			try
			{
				var model = await _dbContext.ModelTbs.FirstOrDefaultAsync(m => m.Id == id);

				if (model == null)
				{
					throw new KeyNotFoundException($"Модель с ID {id} не найдена");
				}

				var oldState = model.LinkState;
				model.LinkState = linkState;
				await _dbContext.SaveChangesAsync();

				_logger.Info(
					"Обновлено состояние ссылки модели {Id}: {OldState} → {NewState}",
					id, oldState, linkState);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Ошибка обновления состояния ссылки модели {Id}", id);
				throw;
			}
		}

		#endregion
	}

}
