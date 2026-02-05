using KameraData.Data.Dtos;
using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_OnboardingVideoService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
		Task<OnboardingVideo?> GetModelsAsync(int? lastId);
		Task<OnboardingVideo?> GetPartsAsync(int? lastId);

		// Issues
		Task<Issue?> GetIssueAsync(int issueId);
		Task<Issue> RegisterStockReportAsync(int userId, int stockId);
		Task<Issue> RegisterPartReportAsync(int userId, int partId);
		Task<Issue> RegisterSimilarReportAsync(int userId, int jobId);
		Task<Issue> RegisterModelReportAsync(int userId, int jobId, int modelId);
		Task<Issue?> DetailIssueAsync(int issueId, int issueType, string? issueDetail);
		// Tips
		Task<TipsResponse> GetTipsForUserAsync(int userId, int? lastVideoTipId, int? lastLinkTipId);
		Task RegisterTipsShowAsync();
	}

}
