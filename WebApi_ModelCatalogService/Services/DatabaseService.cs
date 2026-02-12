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
using System.Data;
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
					.OrderBy(s => s.Confidence)
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
					.Where(s => s.Confidence == confidence)
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
					.OrderBy(s => s.Confidence)
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

        public async Task<BrandModel?> GetOrCreateBrandModelAsync(string? brandTitle, int siteId)
        {
            if (string.IsNullOrWhiteSpace(brandTitle))
                return null;

            try
            {
                // 1. Ищем BrandModel по коду и сайту
                var existing = await _dbContext.BrandModels
                    .Include(bm => bm.Brand)
                    .FirstOrDefaultAsync(bm => bm.Code.Trim().ToUpper() == brandTitle.Trim().ToUpper());

               // if (existing != null)//временно убираем
                    return existing;

                // 2. Ищем или создаём Brand //временно убираем
                //var brand = await _dbContext.Brands
                //    .FirstOrDefaultAsync(b => b.Title == brandTitle);

                //if (brand == null)
                //{
                //    brand = new Brand { Title = brandTitle };
                //    _dbContext.Brands.Add(brand);
                //    await _dbContext.SaveChangesAsync();
                //}

                //// 3. Создаём BrandModel
                //var brandModel = new BrandModel
                //{
                //    Code = brandTitle,
                //    BrandId = brand.Id,
                //    SiteId = siteId,
                //    Cnt = 0
                //};

                //_dbContext.BrandModels.Add(brandModel);
                //await _dbContext.SaveChangesAsync();

                //return brandModel;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка GetOrCreateBrandModelAsync для {BrandTitle} / SiteId={SiteId}", brandTitle, siteId);
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
        ////////////////////////////////////////////для googleupdater//////////////////////////////////////////
        /// <summary>
        /// Получить модель по ключу (SiteId + BrandModelId + CleanedModel).
        /// </summary>
        public async Task<ModelTb?> GetModelBySiteAndKeyAsync(
            int siteId,
            int brandModelId,
            string cleanedModel,
            bool includeBrandModel = false,
            bool includeSite = false)
        {
            try
            {
                var query = _dbContext.ModelTbs
                    .Where(m => m.SiteId == siteId &&
                                m.BrandModelId == brandModelId &&
                                m.CleanedModel == cleanedModel);

                if (includeBrandModel)
                {
                    query = query.Include(m => m.BrandModel!)
                                 .ThenInclude(bm => bm.Brand);
                }

                if (includeSite)
                {
                    query = query.Include(m => m.Site);
                }

                var model = await query.FirstOrDefaultAsync();

                _logger.Debug(
                    "Получена модель по ключу SiteId={SiteId}, BrandModelId={BrandModelId}, CleanedModel={CleanedModel}",
                    siteId, brandModelId, cleanedModel);

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Ошибка получения модели по ключу SiteId={SiteId}, BrandModelId={BrandModelId}, CleanedModel={CleanedModel}",
                    siteId, brandModelId, cleanedModel);
                throw;
            }
        }

        /// <summary>
        /// Создать новую модель.
        /// </summary>
        //public async Task<int> InsertModelAsync(ModelTb model)
        //{
        //    try
        //    {
        //        _dbContext.ModelTbs.Add(model);
        //        await _dbContext.SaveChangesAsync();

        //        _logger.Info("Создана новая модель Id={Id}, Title={Title}", model.Id, model.Title);
        //        return model.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Ошибка при создании модели Title={Title}", model.Title);
        //        throw;
        //    }
        //}
        public async Task<int> InsertModelAsync(ModelTb model)
        {
            _logger.Info("InsertModelAsync started Title={Title}", model.Title);
            try
            {
                var conn = (SqlConnection)_dbContext.Database.GetDbConnection();
                var shouldClose = conn.State != ConnectionState.Open;
                if (shouldClose)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
								EXEC [dbo].[sp_InsertModel]
									@BrandModelId,
									@CleanedModel,
									@CpCounter,
									@Description,
									@Link,
									@LinkState,
									@PartCounter,
									@SiteId,
									@Title,
									@Token";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@BrandModelId",
     model.BrandModelId.HasValue ? model.BrandModelId.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CleanedModel",
                    (object?)model.CleanedModel ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CpCounter",
                    model.CpCounter.HasValue ? model.CpCounter.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description",
                    (object?)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Link",
                    (object?)model.Link ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LinkState", model.LinkState ?? 0);
                cmd.Parameters.AddWithValue("@PartCounter",
                    model.PartCounter.HasValue ? model.PartCounter.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SiteId",
                    model.SiteId.HasValue ? model.SiteId.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Title",
                    (object?)model.Title ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Token",
                    (object?)model.Token ?? DBNull.Value);


                var idObj = await cmd.ExecuteScalarAsync();
                if (shouldClose)
                    await conn.CloseAsync();

                var id = Convert.ToInt32(idObj);
                model.Id = id;

                _logger.Info("Создана новая модель Id={Id}, Title={Title}", model.Id, model.Title);
                return model.Id;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при создании модели Title={Title}", model.Title);
                throw;
            }
        }
        /// <summary>
        /// Сохранить запись в истории изменения ссылок модели.
        /// </summary>
        public async Task InsertModelLinkHistoryAsync(ModelLinkHistory history)
        {
            try
            {
                _dbContext.ModelLinkHistories.Add(history);
                await _dbContext.SaveChangesAsync();

                _logger.Info("Добавлена запись в history для ModelId={ModelId}", history.ModelId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при добавлении записи history для ModelId={ModelId}", history.ModelId);
                throw;
            }
        }

        /// <summary>
        /// Обновить ссылку модели с записью в history и установкой LinkState = 1.
        /// </summary>
        public async Task UpdateModelLinkAsync(int id, string newLink, string source)
        {
            try
            {
                var model = await _dbContext.ModelTbs.FirstOrDefaultAsync(m => m.Id == id);
                if (model == null)
                    throw new KeyNotFoundException($"Модель с ID {id} не найдена");

                var oldLink = model.Link;

                // если ссылка реально изменилась — пишем history
                if (!string.Equals(oldLink, newLink, StringComparison.OrdinalIgnoreCase))
                {
                    var history = new ModelLinkHistory
                    {
                        ModelId = id,
                        OldLink = oldLink,
                        ChangedAt = DateTime.UtcNow,
                        Source = source
                    };

                    _dbContext.ModelLinkHistories.Add(history);
                }

                model.Link = newLink;
                model.LinkState = 1;

                await _dbContext.SaveChangesAsync();

                _logger.Info(
                    "Обновлена ссылка модели {Id}: {OldLink} → {NewLink}, LinkState=1",
                    id, oldLink, newLink);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка обновления ссылки модели {Id}", id);
                throw;
            }
        }


        public async Task<List<ModelLinkHistory>> GetModelLinkHistoryAsync(int modelId, int top = 50)
        {
            try
            {
                if (top <= 0)
                    top = 50;

                return await _dbContext.ModelLinkHistories
                    .Where(h => h.ModelId == modelId)
                    .OrderByDescending(h => h.ChangedAt)
                    .Take(top)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении history для модели {ModelId}: " + ex, modelId);
                return new List<ModelLinkHistory>();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }

}
