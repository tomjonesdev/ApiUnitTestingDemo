using ApiUnitTestingDemo.Controllers;
using ApiUnitTestingDemo.Services;
using Microsoft.Extensions.Logging;

namespace ApiUnitTestingDemo.Tests
{
    public class WeatherForecastControllerTests
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly WeatherForecastController _sut;

        public WeatherForecastControllerTests()
        {
            _logger = Substitute.For<ILogger<WeatherForecastController>>();
            _weatherForecastService = Substitute.For<IWeatherForecastService>();
            _sut = new WeatherForecastController(_logger, _weatherForecastService);
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static readonly IEnumerable<object[]> TestData = new[]
        {
            new object[] { "W1" },
            new object[] { "W3" },
            new object[] { "SW15" },
            new object[] { "NW10" },
        };

        private static readonly WeatherForecast[] WeatherForecasts
            = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_FiveDayForecast_ReturnsFiveItems(string districtCode)
        {
            // Arrange
            _weatherForecastService
                .GetData(districtCode)
                .Returns(WeatherForecasts);

            // Act
            var result = await _sut.Get(districtCode);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task Get_UnrecognisedDistrictCode_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotSupportedException>(
                async () => await _sut.Get("test"));

            // Assert
            Assert.Equal("No data available for test", exception.Message);
        }

        [Fact]
        public async Task Get_FiveDayForecast_ReturnsNextFiveDaysData()
        {
            // Arrange
            var dateNow = DateTime.Now;
            var firstDay = dateNow.AddDays(1).Day;
            var lastDay = dateNow.AddDays(5).Day;

            _weatherForecastService
                .GetData("W1")
                .Returns(WeatherForecasts);

            // Act
            var result = await _sut.Get("W1");

            // Assert
            Assert.Equal(firstDay, result.First().Date.Day);
            Assert.Equal(lastDay, result.Last().Date.Day);
        }

        [Fact]
        public async Task Get_LogInformation_MessageLoggedCorrectly()
        {
            // Arrange
            _weatherForecastService
                .GetData("W1")
                .Returns(WeatherForecasts);

            // Act
            await _sut.Get("W1");

            // Assert
            _logger
                .Received(1)
                .LogInformation("Get method was called");
        }

        [Fact]
        public async Task Get_NoDistrictCode_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _sut.Get(""));
        }
    }
}
