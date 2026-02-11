using KameraData.Data.Dtos;
using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_ModelCatalogService.Controllers;

namespace WebApi_ModelCatalogService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {

        #region Models

        /// <summary>
        /// Получить модель по ID с опциональными Include
        /// </summary>
        Task<ModelTb?> GetModelByIdAsync(int id, bool includeBrandModel = false, bool includeSite = false);

        /// <summary>
        /// Получить модели по списку ID с опциональными Include
        /// </summary>
        Task<List<ModelTb>> GetModelsByIdsAsync(int[] ids, bool includeBrandModel = false, bool includeSite = false);

        /// <summary>
        /// Обновить состояние ссылки модели
        /// </summary>
        Task UpdateModelLinkStateAsync(int id, int linkState);

        Task<ModelTb?> GetModelBySiteAndKeyAsync(int siteId, int brandModelId, string cleanedModel,
                                         bool includeBrandModel = false, bool includeSite = false);
        Task<int> InsertModelAsync(ModelTb model);
        Task UpdateModelLinkAsync(int id, string newLink, string source);
        Task InsertModelLinkHistoryAsync(ModelLinkHistory history);

        Task<List<ModelLinkHistory>> GetModelLinkHistoryAsync(int modelId, int top = 50);
        #endregion

        #region Brands

        Task<Brand?> GetBrandByIdAsync(int id);
        Task<List<Brand>> GetBrandsByIdsAsync(int[] ids);
        Task<List<Brand>> GetAllBrandsAsync();
        Task<List<Brand>> SearchBrandsByTitleAsync(string query);
        Task<Brand?> GetBrandByTitleAsync(string title);
        Task<List<BrandWithCountDto>> GetPopularBrandsAsync(int top = 10);
        Task<int> GetBrandModelsCountAsync(int brandId);
        #endregion

        #region BrandModel
        Task<BrandModel?> GetOrCreateBrandModelAsync(string? brandTitle, int siteId);

        #endregion

        #region Sites

        Task<Site?> GetSiteByIdAsync(int id);
        Task<List<Site>> GetSitesByIdsAsync(int[] ids);
        Task<List<Site>> GetAllSitesAsync();
        Task<List<Site>> GetSitesByConfidenceAsync(string confidence);
        Task<List<Site>> SearchSitesByTitleAsync(string query);
       
        #endregion




    }

}
