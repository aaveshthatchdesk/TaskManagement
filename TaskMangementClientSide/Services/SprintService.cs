using System.Net.Http;
using System.Net.Http.Json;
using Task.Application.DTOs;

namespace TaskMangementClientSide.Services
{
    public class SprintService
    {
        private readonly HttpClient _http;
        public SprintService(HttpClient http)
        {
            _http = http;
        }
        public async Task<bool>AddSprintAsync(int ProjectId,SprintDto sprint)
        {
            var response=await _http.PostAsJsonAsync($"api/sprint/project/{ProjectId}",sprint);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<SprintDto>>GetAllSprintAsync()
        {
            var sprints = await _http.GetFromJsonAsync<List<SprintDto>>("api/sprint");
            return sprints ?? new List<SprintDto>();
        }
        public async Task<SprintDto?> GetSprintByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<SprintDto>($"api/sprint/{id}");
        }
        public async Task<bool> UpdateSprintAsync(int id, SprintDto sprint)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/sprint/{id}", sprint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
