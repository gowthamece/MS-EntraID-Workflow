using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace EntraID_Workflow.Web;

public class AppRegistrationApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenAcquisition _tokenAcquisition;

    public AppRegistrationApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ITokenAcquisition tokenAcquisition)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _tokenAcquisition = tokenAcquisition;
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "api://f90901cd-4501-4999-b6ca-aad84c6232a6/UserRole.Read" });

        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public async Task CreateAppRegistrationAsync(AppRegistration appRegistration, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();

        await _httpClient.PostAsJsonAsync("/api/appregistration", appRegistration, cancellationToken);
    }
}

public record AppRegistration
{
    public int Id { get; init; } = 0;
    public string AppName { get; init; } = string.Empty;
    public string OwnerEmail { get; init; } = string.Empty;
    public int AppTypeId { get; init; } = 0;
    public AppType AppType { get; init; } = new();
    public string RedirectUrl { get; init; } = string.Empty;
    public int StatusId { get; init; } = 0;
    public Status Status { get; init; } = new();

    public AppRegistration() { }

    public AppRegistration(string appName, string ownerEmail, int appTypeId, AppType appType, string redirectUrl)
    {
        Id = 0;
        AppName = appName;
        OwnerEmail = ownerEmail;
        AppTypeId = appTypeId;
        AppType = appType;
        RedirectUrl = redirectUrl;
        StatusId = 0;
        Status = new Status { Id = 0, Name = "Submitted" };
    }
}

public record AppType
{
    public int Id { get; init; } = 0;
    public string Name { get; init; } = string.Empty;
}

public record Status
{
    public int Id { get; init; } = 0;
    public string Name { get; init; } = string.Empty;
}