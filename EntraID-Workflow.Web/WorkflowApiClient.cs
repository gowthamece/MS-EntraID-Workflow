namespace EntraID_Workflow.Web;

public class WorkflowApiClient
{
    private readonly HttpClient _httpClient;

    public WorkflowApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Workflow>> GetWorkflowsAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<Workflow>>("/api/workflows", cancellationToken) ?? new List<Workflow>();
    }

    public async Task<Workflow?> GetWorkflowByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<Workflow>($"/api/workflows/{id}", cancellationToken);
    }

    public async Task CreateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsJsonAsync("/api/workflows", workflow, cancellationToken);
    }

    public async Task UpdateWorkflowAsync(int id, Workflow workflow, CancellationToken cancellationToken = default)
    {
        await _httpClient.PutAsJsonAsync($"/api/workflows/{id}", workflow, cancellationToken);
    }

    public async Task DeleteWorkflowAsync(int id, CancellationToken cancellationToken = default)
    {
        await _httpClient.DeleteAsync($"/api/workflows/{id}", cancellationToken);
    }
}

public record Workflow(int Id, string Name, string Description); 