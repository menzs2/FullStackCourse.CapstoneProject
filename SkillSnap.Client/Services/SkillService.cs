using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client
{
    public class SkillService
    {
        private readonly HttpClient _httpClient;

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            var response = await _httpClient.GetAsync("api/skill");
            response.EnsureSuccessStatusCode();
            var skills = await response.Content.ReadFromJsonAsync<List<Skill>>();
            if (skills == null || !skills.Any())
            {
                throw new Exception("No skills found.");
            }
            return skills;
        }

        public async Task CreateSkillAsync(Skill skill)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/skill", skill);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to create skill.", ex);
            }
        }
    }
}
