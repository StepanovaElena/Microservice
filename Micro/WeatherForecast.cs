using Microsoft.AspNetCore.Mvc;
using System;

namespace Micro
{
    public class WeatherForecast
    {
        [FromQuery]
        public DateTime Date { get; set; }
        [FromQuery]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
