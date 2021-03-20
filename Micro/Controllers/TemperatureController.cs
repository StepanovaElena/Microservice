using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Controllers
{
    [Route("api/temperature")]
    [ApiController]
    public class TemperatureController : Controller
    {
        private readonly ValuesHolder holder;

        public TemperatureController(ValuesHolder holder)
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
            var isDateRange = dateFrom.ToString().Length > 0 && dateTo.ToString().Length > 0;

            if (isDateRange && dateTo < dateFrom)
            {
                return BadRequest();
            }

            var values = isDateRange ? holder.Values.Where(v => v.Date > dateFrom && v.Date < dateTo) : holder.Values;
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

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] DateTime date)
        {
            holder.Values = holder.Values.Where(v => v.Date != date).ToList();
            return Ok();
        }

        [HttpDelete("delete/range")]
        public IActionResult DeleteRange([FromQuery] DateTime dateFrom, DateTime dateTo)
        {
            if (dateTo < dateFrom)
            {
                return BadRequest();
            }

            holder.Values.RemoveAll(v => v.Date > dateFrom && v.Date < dateTo);
            return Ok();
        }
    }
}
