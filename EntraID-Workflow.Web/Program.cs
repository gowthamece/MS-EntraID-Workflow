using EntraID_Workflow.Web;
using EntraID_Workflow.Web.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });
builder.Services.AddHttpClient<WorkflowApiClient>(client =>
{    client.BaseAddress = new("https+http://apiservice"); // Replace with your API base URL
});

builder.Services.AddSingleton<KeyVaultService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var keyVaultUrl = configuration["AzureAd:KeyVaultUrl"]
                      ?? throw new InvalidOperationException("AzureAd:KeyVaultUrl is not configured.");
    return new KeyVaultService(keyVaultUrl);
});

// Register IHttpContextAccessor for dependency injection
//builder.Services.AddHttpContextAccessor();

// Ensure tokens are saved for retrieval
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(configureMicrosoftIdentityOptions =>
    {
        builder.Configuration.Bind("AzureAd", configureMicrosoftIdentityOptions);
     //   configureMicrosoftIdentityOptions.SaveTokens = true; // Save tokens for retrieval
    })
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { builder.Configuration["DownstreamApi:Scopes"] ?? string.Empty })
    .AddDownstreamApi("WeatherList", builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy 
    options.FallbackPolicy = options.DefaultPolicy;
});

//builder.Services.ConfigureApplicationCookie(configureCookieAuthenticationOption =>
//{
//    configureCookieAuthenticationOption.Cookie.SameSite = SameSiteMode.None;
//    configureCookieAuthenticationOption.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    configureCookieAuthenticationOption.Cookie.Name = "BlazorApp01";
//});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

var environment = builder.Environment.EnvironmentName;
string clientSecret;

if (environment == "Development")
{
    // Read client secret from local secret.json file in development
    clientSecret = builder.Configuration["AzureAd:ClientSecret"] ?? throw new InvalidOperationException("AzureAd:ClientSecret is not configured.");
}
else
{
    // Use Key Vault for staging and production
    var keyVaultService = new KeyVaultService(builder.Configuration["AzureAd:KeyVaultUrl"] ?? throw new InvalidOperationException("AzureAd:ClientSecret is not configured."));
    clientSecret = await keyVaultService.GetSecretAsync("ClientSecret");
}

builder.Services.Configure<MicrosoftIdentityOptions>(options =>
{
    builder.Configuration.Bind("AzureAd", options);
    options.ClientSecret = clientSecret;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

app.UseOutputCache();
app.MapBlazorHub();
// Wrap Razor components in CascadingAuthenticationState to provide AuthenticationState
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
// .WrapInCascadingAuthenticationState();

app.MapDefaultEndpoints();

app.Run();
