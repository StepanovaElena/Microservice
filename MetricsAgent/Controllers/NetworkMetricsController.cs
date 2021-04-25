using AutoMapper;
using MetricsAgent.DAL;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/network")]
    [ApiController]
    public class NetworkMetricsController : Controller
    {
        private readonly ILogger<NetworkMetricsController> _logger;
        private readonly IMapper _mapper;
        private readonly INetworkMetricsRepository _repository;

        public NetworkMetricsController(ILogger<NetworkMetricsController> logger, INetworkMetricsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

            _logger.LogInformation("NLog встроен в NetworkMetricsController");
        }

        /// <summary>
        /// Получение Network метрик в заданный промежуток времени.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetrics([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsByPercentile : fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(fromTime, toTime);

            var response = new AllNetworkMetricsResponse()
            {
                Metrics = new List<NetworkMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
