using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client
{
    public class PortfolioService
    {
        private readonly HttpClient _httpClient;

        public PortfolioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PortfolioUser>> GetPortfoliosAsync()
        {
            var response = await _httpClient.GetAsync("api/portfolio");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<PortfolioUser>>();
            }
            return new List<PortfolioUser>();
        }

        public async Task<PortfolioUser?> GetPortfolioAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/portfolio/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PortfolioUser>();
            }
            return null;
        }
    }
}
