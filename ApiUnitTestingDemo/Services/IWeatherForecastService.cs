namespace ApiUnitTestingDemo.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetData(string districtCode);
    }
}
