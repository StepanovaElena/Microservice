using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsManager.Enums;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]

    public class RamMetricsController : Controller
    {
        private readonly ILogger<RamMetricsController> _logger;

        public RamMetricsController(ILogger<RamMetricsController> logger)
        {
            _logger = logger;
            _logger.LogInformation("NLog встроен в RamMetricsController");
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");


            return Ok();
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");

            return Ok();
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAllCluster : fromTime = {fromTime}; toTime = {toTime}");

            return Ok();
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAllCluster : fromTime = {fromTime}; toTime = {toTime}; percentile = {percentile}");

            return Ok();
        }
    }
}
