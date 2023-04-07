using Microsoft.AspNetCore.Mvc;
using BlazorApp.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BlazorApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetForecastAsync()
        {
            var svc = new WeatherForecastService();
            return new OkObjectResult(await svc.GetForecastAsync(DateOnly.FromDateTime(DateTime.Now)));
        }
    }
}
