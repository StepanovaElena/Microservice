using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Controllers
{
    [Route("api/weather-forecast")]
    [ApiController]
    public class WeatherForecastController : Controller
    {
        private readonly ValuesHolder holder;

        public WeatherForecastController(ValuesHolder holder)
        {
            this.holder = holder;
        }

        [HttpPost("create")]
        public IActionResult Create([FromQuery] WeatherForecast newIndicator)
        {
            holder.Values.Add(newIndicator);
            return Ok();
        }

        [HttpGet("read")]
        public IActionResult Read([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var isDateRange = dateFrom.HasValue && dateTo.HasValue;
            var values = holder.Values;

            if (isDateRange && dateTo < dateFrom)
            {
                return BadRequest();
            }

            if (dateFrom.HasValue && !dateTo.HasValue)
            {
                values = values.Where(v => v.Date >= dateFrom).ToList();
            } 
            else if (!dateFrom.HasValue && dateTo.HasValue)
            {
                values = values.Where(v => v.Date <= dateTo).ToList();
            }
            else if (isDateRange)
            {
                values = holder.Values.Where(v => v.Date >= dateFrom && v.Date <= dateTo).ToList();
            }
           
            return Ok(values);
        }

        [HttpPut("update")]
        public IActionResult Update([FromQuery] WeatherForecast newIndicator)
        {
            for (int i = 0; i < holder.Values.Count; i++)
            {
                if (holder.Values[i].Date == newIndicator.Date)
                    holder.Values[i].TemperatureC = newIndicator.TemperatureC;
            }

            return Ok();
        }
       
        [HttpDelete("delete/range")]
        public IActionResult DeleteRange([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var isDateRange = dateFrom.HasValue && dateTo.HasValue;

            if (isDateRange && dateTo < dateFrom)
            {
                return BadRequest();
            }

            holder.Values.RemoveAll(v => v.Date >= dateFrom && v.Date <= dateTo);
            return Ok();
        }
    }
}
