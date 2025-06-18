using DataLayer;

namespace BusinessLayer
{
    public interface IServiceLayer
    {
        IEnumerable<WeatherForecast> GetWeatherForecasts();
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync();

        Task<bool> AddWeatherForecastAsync(WeatherForecast forecast);
    }
}
