namespace DataLayer
{
    public interface IWeatherForecast
    {
        IEnumerable<WeatherForecast> GetSummariesForecasts();
        IEnumerable<WeatherForecast> GenerateForecasts();
        IEnumerable<WeatherForecast> AssignSummaries(IEnumerable<WeatherForecast> forecasts);
        Task<IEnumerable<WeatherForecast>> GenerateForecastsAsync();
        Task<IEnumerable<WeatherForecast>> AssignSummariesAsync(IEnumerable<WeatherForecast> forecasts);

        Task<bool> AddForecastAsync(WeatherForecast forecast);
    }
}