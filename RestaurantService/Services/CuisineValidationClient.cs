using System.Net.Http.Json;

namespace RestaurantService.Services;

public class CuisineValidationClient : ICuisineValidationClient
{
    private readonly HttpClient _client;

    public CuisineValidationClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Gateway");
    }

    public async Task<bool> ExistsAsync(Guid cuisineId)
    {
        var res = await _client.GetAsync($"/api/cuisines/{cuisineId}/exists");
        if (!res.IsSuccessStatusCode)
            return false;
        var payload = await res.Content.ReadFromJsonAsync<ExistsResponse?>();
        return payload?.Exists ?? false;
    }

    private class ExistsResponse { public bool Exists { get; set; } }
}
