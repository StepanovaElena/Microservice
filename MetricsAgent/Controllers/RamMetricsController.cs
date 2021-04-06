using MetricsAgent.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RamMetricsController> _logger;
        private readonly IRamMetricsRepository _repository;

        public RamMetricsController(ILogger<RamMetricsController> logger, IRamMetricsRepository repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogInformation("NLog встроен в RamMetricsController");
        }

        [HttpGet("available")]
        public IActionResult GetAvailableRam()
        {
            _logger.LogDebug("GetAvailableRam : без параметров");

            return Ok();
        }
    }
}
