using KameraData.Data.Models;
using Newtonsoft.Json;

namespace WebApi_JobService
{
    public class UserServiceClient
    {
        private readonly HttpClient _httpClient;
        public UserServiceClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<User> GetUserByTelegramIdAsync(int telegramId)
        {
            var response = await _httpClient.GetAsync($"api/users/getuserbyTg/{telegramId}");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(content);
        }
    }
}
