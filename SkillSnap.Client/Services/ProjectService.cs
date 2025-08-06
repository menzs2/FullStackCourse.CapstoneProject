using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            var response = await _httpClient.GetAsync("api/project");
            response.EnsureSuccessStatusCode();
            var projects = await response.Content.ReadFromJsonAsync<List<Project>>();
            if (projects == null || !projects.Any())
            {
                throw new Exception("No projects found.");
            }
            return projects;
        }

        public async Task CreateProjectAsync(Project project)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/project", project);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to create project.", ex);
            }
        }
    }
}
