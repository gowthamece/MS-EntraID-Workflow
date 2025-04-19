using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace EntraID_Workflow.Web.Components.Pages.WeatherPages
{
    public class WeatherBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }
        [Inject]
        IDownstreamApi _downstreamApi { get; set; }
        protected IEnumerable<WeatherModel> weatherList { get; set; } = new List<WeatherModel>();
        //  public WeatherModel[] WeatherForecasts { get; set; } = Array.Empty<WeatherModel>();
        protected override async Task OnInitializedAsync()
        {
            await GetToDoListSerice();
        }

        private async Task EnsureAuthenticatedAsync()
        {
            try
            {
                // Ensure the user is authenticated
                ConsentHandler.ChallengeUser(null); // Updated to use ChallengeUser instead of ChallengeUserAsync
            }
            catch (Exception ex)
            {
                // Handle authentication exceptions
                throw new InvalidOperationException("Authentication failed.", ex);
            }
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        private async Task GetToDoListSerice()
        {
            try
            {
                // Ensure the user is authenticated before making the API call
            //    await EnsureAuthenticatedAsync();

                weatherList = await _downstreamApi.GetForUserAsync<IEnumerable<WeatherModel>>("WeatherList");
            }
            catch (MsalUiRequiredException ex)
            {
                // Handle specific MSAL exceptions
                throw new InvalidOperationException("User interaction required for authentication.", ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
            }
            finally
            {
                // Cleanup if necessary
            }
        }
    }
}
