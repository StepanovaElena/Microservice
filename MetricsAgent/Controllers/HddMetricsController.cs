using MetricsAgent.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HddMetricsController> _logger;
        private readonly IHddMetricsRepository _repository;

        public HddMetricsController(ILogger<HddMetricsController> logger, IHddMetricsRepository repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogInformation("NLog встроен в HddMetricsController");
        }


        [HttpGet("left")]
        public IActionResult GetSpaceLeft()
        {
            _logger.LogDebug("GetSpaceLeft : без параметров");

            return Ok();
        }
    }
}
