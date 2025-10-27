using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RestaurantService.Services
{
    public class CuisineClient : ICuisineClient
    {
        private readonly HttpClient _http;

        public CuisineClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CuisineExistsAsync(Guid cuisineId)
        {
            var resp = await _http.GetAsync($"api/cuisines/{cuisineId}/exists");
            if (!resp.IsSuccessStatusCode)
                return false;

            var obj = await resp.Content.ReadFromJsonAsync<ExistsResponse?>();
            return obj?.Exists ?? false;
        }

        private class ExistsResponse { public bool Exists { get; set; } }
    }
}
