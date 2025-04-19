namespace EntraID.Workflow.ApiService.Models
{
    public class WeatherForecast
    {
        private DateOnly dateOnly;
        private int v1;
        private string v2;

        public WeatherForecast(DateOnly dateOnly, int v1, string v2)
        {
            this.dateOnly = dateOnly;
            this.v1 = v1;
            this.v2 = v2;
        }

        public DateOnly Date { get; init; }
        public int TemperatureC { get; init; }
        public string? Summary { get; init; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    }
}
