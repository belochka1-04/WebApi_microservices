using KameraData.Data.Dtos;
using Microsoft.Extensions.Options;
using WebApi_MaskService.Interface;

namespace WebApi_MaskService.Httpclient
{
    public class HttpJobClient : IJobClient
    {
        private readonly HttpClient _httpClient;
        private readonly JobServiceOptions _options;

        public HttpJobClient(HttpClient httpClient, IOptions<JobServiceOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<JobDto?> GetJobByIdAsync(int jobId)
        {
            var path = _options.GetJobDtoPath.Replace("{id}", jobId.ToString());

            var response = await _httpClient.GetAsync(path);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<JobDto>();
        }
    }

}
