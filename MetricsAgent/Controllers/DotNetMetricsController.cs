using AutoMapper;
using MetricsAgent.DAL;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/dotnet")]
    [ApiController]
    public class DotNetMetricsController : Controller
    {
        private readonly ILogger<DotNetMetricsController> _logger;
        private readonly IDotNetMetricsRepository _repository;
        private readonly IMapper _mapper;

        public DotNetMetricsController(ILogger<DotNetMetricsController> logger, IDotNetMetricsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _logger.LogInformation("NLog встроен в DotNetMetricsController");
        }

        /// <summary>
        /// Получение количества ошибок DotNet метрик в заданный промежуток времени.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <returns>Общее количество ошибок.</returns>
        [HttpGet("errors-count/from/{fromTime}/to/{toTime}")]
        public IActionResult GetErrorsCount([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsByPercentile : fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(fromTime, toTime);

            var response = metrics.Count();           

            return Ok(response);
        }
        
        /// <summary>
        /// Получение DotNet метрик в заданный промежуток времени.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetrics([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetrics : fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(fromTime, toTime);

            var response = new AllDotNetMetricsResponse()
            {
                Metrics = new List<DotNetMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
