using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using DataLayer;
namespace WebLogging.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IServiceLayer _serviceLayer;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceLayer serviceLayer)
        {
            _logger = logger;
            _serviceLayer = serviceLayer;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<DataLayer.WeatherForecast> Get()
        {
            return _serviceLayer.GetWeatherForecasts();
        }

        [HttpGet("async", Name = "GetWeatherForecastAsync")]
        public async Task<IEnumerable<DataLayer.WeatherForecast>> GetAsync()
        {
            return await _serviceLayer.GetWeatherForecastsAsync();
        }

        [HttpPost("async", Name = "PostWeatherForecastAsync")]
        public async Task<IActionResult> PostAsync([FromBody] DataLayer.WeatherForecast forecast)
        {
            if (forecast == null)
            {
                return BadRequest("Invalid forecast data.");
            }

            var result = await _serviceLayer.AddWeatherForecastAsync(forecast);
            return Ok(result);
        }
    }
}
