using EntraID_Workflow.Web;
using EntraID_Workflow.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
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

builder.Services.ConfigureApplicationCookie(configureCookieAuthenticationOption =>
{
    configureCookieAuthenticationOption.Cookie.SameSite = SameSiteMode.None;
    configureCookieAuthenticationOption.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    configureCookieAuthenticationOption.Cookie.Name = "BlazorApp01";
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.UseAntiforgery();

app.UseOutputCache();

// Wrap Razor components in CascadingAuthenticationState to provide AuthenticationState
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
// .WrapInCascadingAuthenticationState();

app.MapDefaultEndpoints();

app.Run();
