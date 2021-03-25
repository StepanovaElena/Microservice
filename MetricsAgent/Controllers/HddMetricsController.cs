using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : Controller
    {
        [HttpGet("left")]
        public IActionResult GetSpaceLeft()
        {
            return Ok();
        }
    }
}
