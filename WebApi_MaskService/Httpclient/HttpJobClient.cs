using KameraData.Data.Dtos;
using WebApi_MaskService.Interface;

namespace WebApi_MaskService.Httpclient
{
    public class HttpJobClient : IJobClient
    {
        private readonly HttpClient _httpClient;

        public HttpJobClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JobDto?> GetJobByIdAsync(int jobId)
        {
            var response = await _httpClient.GetAsync($"/api/jobs/getdtojob/{jobId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<JobDto>();
        }
    }

}
