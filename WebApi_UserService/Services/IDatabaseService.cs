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
        Task<User> GetUserByTgAsync(int Id);

        Task SaveDefaultUserToBDAsync(int? telegramId, string prefersTelegram);

        Task SaveUserToBDAsync(User user);

        Task UpdateUserInBDAsync(User user);

        Task<StockCred> GetStockCredById(int stockId);

        Task<List<UserStock>> GetUserStocksByStockCredAsync(int stockCredId, int userId);

        //для бота
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateOrGetUserAsync(long telegramId, int? referralUserId);
        Task<bool> UpdateUserModeAsync(int userId, byte mode);

        Task<bool> ExtendProAsync(int userId, int months);

        Task<bool> UpdateLastVideoTipAsync(int userId, int tipId);
        Task<bool> UpdateLastLinkTipAsync(int userId, int tipId);
        Task<bool> UpdateLastRepairVideoAsync(int userId, int videoId);
        Task<bool> UpdateLastWarehouseVideoAsync(int userId, int videoId);

    }

}
