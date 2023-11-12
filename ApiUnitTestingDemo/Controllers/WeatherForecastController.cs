using ApiUnitTestingDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiUnitTestingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] SupportedRegions = new[]
        {
            "W1", "W3", "SW15", "NW10"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string districtCode)
        {
            if (string.IsNullOrWhiteSpace(districtCode))
            {
                throw new ArgumentNullException(nameof(districtCode));
            }

            _logger.LogInformation("Get method was called");

            if (!SupportedRegions.Contains(districtCode))
            {
                throw new NotSupportedException("No data available for " + districtCode);
            }

            return await _weatherForecastService.GetData(districtCode);
        }
    }
}