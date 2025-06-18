using DataLayer;

namespace BusinessLayer
{
    public class ServiceLayer : IServiceLayer
    {
        private readonly IWeatherForecast _weatherForecast;

        public ServiceLayer(IWeatherForecast weatherForecast)
        {
            _weatherForecast = weatherForecast;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [LogMessage("Fetching weather forecasts.")]
        public IEnumerable<WeatherForecast> GetWeatherForecasts()
        {
            // Step 1: Generate forecasts
            var forecasts = _weatherForecast.GenerateForecasts();

            // Step 2: Assign summaries to the forecasts
            return _weatherForecast.AssignSummaries(forecasts);
        }
        [LogMessage("Fetching weather forecasts asynchronously.")]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            // Step 1: Generate forecasts asynchronously
            var forecasts = await _weatherForecast.GenerateForecastsAsync();

            // Step 2: Assign summaries to the forecasts asynchronously
            return await _weatherForecast.AssignSummariesAsync(forecasts);
        }
        [LogMessage("Adding a new weather forecast.")]
        public async Task<bool> AddWeatherForecastAsync(WeatherForecast forecast)
        {
            // Delegate the operation to the data layer
            return await _weatherForecast.AddForecastAsync(forecast);
        }



    }
}
