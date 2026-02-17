using KameraData.Data.Dtos;
using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_GoogleSearchTemplatesService.Controllers;

namespace WebApi_GoogleSearchTemplatesService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
		// ===== google_serp_raw =====

		/// <summary>
		/// Вставить одну запись SERP и вернуть её Id.
		/// </summary>
		Task<int> InsertGoogleSerpRawAsync(GoogleSerpRaw serp);

		/// <summary>
		/// Батчевая вставка SERP-записей, вернуть список Id.
		/// </summary>
		Task<IReadOnlyList<int>> InsertGoogleSerpRawBatchAsync(IEnumerable<GoogleSerpRaw> serps);


		// ===== site_templates =====

		/// <summary>
		/// Получить активные шаблоны сайтов.
		/// </summary>
		Task<List<SiteTemplate>> GetActiveSiteTemplatesAsync();

		/// <summary>
		/// Получить все шаблоны сайтов.
		/// </summary>
		Task<List<SiteTemplate>> GetAllSiteTemplatesAsync();

        Task<List<SiteTemplate>> GetActiveSiteTemplatesWithTitleRulesAsync();

        Task<List<SiteTemplateTitleRule>> GetActiveTitleRulesAsync(int siteTemplateId);

        /// <summary>
        /// Получить шаблон сайта по Id.
        /// </summary>
        Task<SiteTemplate?> GetSiteTemplateByIdAsync(int id);

		/// <summary>
		/// Создать новый шаблон сайта.
		/// </summary>
		Task<int> CreateSiteTemplateAsync(SiteTemplate template);

		/// <summary>
		/// Обновить существующий шаблон сайта.
		/// </summary>
		Task UpdateSiteTemplateAsync(SiteTemplate template);


		// ===== GoogleModelRequests =====

		/// <summary>
		/// Получить pending-запросы к Google (Status = 0) пачкой.
		/// </summary>
		Task<List<GoogleModelRequest>> GetPendingGoogleRequestsAsync(int batchSize);

		/// <summary>
		/// Создать новый запрос к Google.
		/// </summary>
		Task<GoogleModelRequest> CreateGoogleModelRequestAsync(string request);

		/// <summary>
		/// Пометить запрос как обработанный (Status = 1, LastCheckedAt = now).
		/// </summary>
		Task MarkGoogleModelRequestProcessedAsync(int id);

		/// <summary>
		/// Обновить статус запроса (произвольное значение Status).
		/// </summary>
		Task UpdateGoogleModelRequestStatusAsync(int id, byte status);


	}

}
