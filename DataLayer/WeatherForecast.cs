namespace DataLayer
{
    public class WeatherForecast :IWeatherForecast
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly List<WeatherForecast> _forecasts = new();
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        [LogMessage("Generating Get Summaries Forecasts.", "WeatherForecast", "CTE")]
        public IEnumerable<WeatherForecast> GetSummariesForecasts()
        {
            var forecasts = GenerateForecasts();
            return AssignSummaries(forecasts);
        }
        [LogMessage("Generating weather forecasts .", "WeatherForecast", "CTE")]
        public IEnumerable<WeatherForecast> GenerateForecasts()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55)
            });
        }
        [LogMessage("Generating Assign Summaries weather forecasts.", "WeatherForecast", "CTE")]
        public IEnumerable<WeatherForecast> AssignSummaries(IEnumerable<WeatherForecast> forecasts)
        {
            return forecasts.Select(forecast =>
            {
                forecast.Summary = Summaries[Random.Shared.Next(Summaries.Length)];
                return forecast;
            });
        }
        [LogMessage("Generating forecasts asynchronously.", "WeatherForecast", "CTE")]
        public async Task<IEnumerable<WeatherForecast>> GenerateForecastsAsync()
        {
            return await Task.Run(() =>
            {
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55)
                });
            });
        }
        [LogMessage("Generating Assign Summaries asynchronously." ,"WeatherForecast", "CTE")]
        public async Task<IEnumerable<WeatherForecast>> AssignSummariesAsync(IEnumerable<WeatherForecast> forecasts)
        {
            return await Task.Run(() =>
            {
                return forecasts.Select(forecast =>
                {
                    forecast.Summary = Summaries[Random.Shared.Next(Summaries.Length)];
                    return forecast;
                });
            });
        }
        [LogMessage("Add forecasts asynchronously." ,"WeatherForecast", "CTE")]
        public async Task<bool> AddForecastAsync(WeatherForecast forecast)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _forecasts.Add(forecast);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }


    }
}
