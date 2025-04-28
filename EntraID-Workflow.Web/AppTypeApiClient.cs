using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace EntraID_Workflow.Web
{
    public class AppTypeApiClient
    {
        private readonly HttpClient _http;
        private readonly ITokenAcquisition _tokenAcquisition;

        public AppTypeApiClient(HttpClient http, ITokenAcquisition tokenAcquisition)
        {
            _http = http;
            _tokenAcquisition = tokenAcquisition;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
                new[] { "api://f90901cd-4501-4999-b6ca-aad84c6232a6/UserRole.Read" });
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<AppType>> GetAppTypesAsync(CancellationToken ct = default)
        {
            await AddAuthHeaderAsync();
            return await _http.GetFromJsonAsync<List<AppType>>("/api/apptype", ct)
                   ?? new List<AppType>();
        }

        public async Task<AppType> CreateAppTypeAsync(AppType newType, CancellationToken ct = default)
        {
            await AddAuthHeaderAsync();
            var resp = await _http.PostAsJsonAsync("/api/apptype", newType, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AppType>(cancellationToken: ct)
                   ?? throw new InvalidOperationException("Empty response");
        }

        public async Task UpdateAppTypeAsync(int id, AppType updatedType, CancellationToken ct = default)
        {
            await AddAuthHeaderAsync();
            var resp = await _http.PutAsJsonAsync($"/api/apptype/{id}", updatedType, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAppTypeAsync(int id, CancellationToken ct = default)
        {
            await AddAuthHeaderAsync();
            var resp = await _http.DeleteAsync($"/api/apptype/{id}", ct);
            resp.EnsureSuccessStatusCode();
        }
    }
}
public record Workflow(int id, string Name);