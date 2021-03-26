using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : Controller
    {
        [HttpGet("available")]
        public IActionResult GetAvailableRam()
        {
            return Ok();
        }
    }
}
