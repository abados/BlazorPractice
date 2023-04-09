using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFileUpload.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get([FromQuery] int page)
        {
            try {
            return Enumerable.Range(page*5-4, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<WeatherForecast>();
            }
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get(bool sortByAscending, string column)
        {
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

            if (column == "Date")
            {
                if (sortByAscending)
                {
                    result = result.OrderBy(x => x.Date);
                }
                else
                {
                    result = result.OrderByDescending(x => x.Date);
                }
            }

            return result;
        }
    }
}