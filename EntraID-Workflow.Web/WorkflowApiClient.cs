using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace EntraID_Workflow.Web;

public class WorkflowApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IDownstreamApi _downstreamApi;
    public WorkflowApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ITokenAcquisition tokenAcquisition, IDownstreamApi downstreamApi)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _tokenAcquisition = tokenAcquisition;
        _downstreamApi = downstreamApi;
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "api://f90901cd-4501-4999-b6ca-aad84c6232a6/UserRole.Read" });

        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public async Task<List<Workflow>> GetWorkflowsAsync(CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        // Use the DownstreamApi to get the access token and make the request
      //  var workflow= await _downstreamApi.GetForUserAsync<IEnumerable<Workflow>>("Workflow");
        return await _httpClient.GetFromJsonAsync<List<Workflow>>("/api/workflows", cancellationToken) ?? new List<Workflow>();
    }

    public async Task<Workflow?> GetWorkflowByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<Workflow>($"/api/workflows/{id}", cancellationToken);
    }

    public async Task CreateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        await _httpClient.PostAsJsonAsync("/api/workflows", workflow, cancellationToken);
    }

    public async Task UpdateWorkflowAsync(int id, Workflow workflow, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        await _httpClient.PutAsJsonAsync($"/api/workflows/{id}", workflow, cancellationToken);
    }

    public async Task DeleteWorkflowAsync(int id, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        await _httpClient.DeleteAsync($"/api/workflows/{id}", cancellationToken);
    }
}

public record Workflow(int Id, string Name, string Description);