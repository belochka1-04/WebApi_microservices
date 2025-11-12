using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_UserService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<List<UserStock>> GetStockById(int userId);
        Task InsertUserStockAsync(UserStock responce);

        Task DeleteUserStocksAsync(int Id);

        Task<int> GetUserCrmAsync(int UserId);

        Task SaveDefaultUserToBDAsync(int? telegramId, string prefersTelegram);

        Task SaveUserToBDAsync(User user);

        Task UpdateUserInBDAsync(User user);

        Task<StockCred> GetStockCredById(int stockId);

        Task<List<UserStock>> GetUserStocksByStockCredAsync(int stockCredId, int userId);

    }

}
