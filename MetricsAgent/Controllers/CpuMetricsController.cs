using AutoMapper;
using MetricsAgent.DAL;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/cpu")]
    [ApiController]

    public class CpuMetricsController : ControllerBase
    {
        private readonly ILogger<CpuMetricsController> _logger;
        private readonly ICpuMetricsRepository _repository;
        private readonly IMapper _mapper;

        public CpuMetricsController(ILogger<CpuMetricsController> logger, ICpuMetricsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

            _logger.LogInformation("NLog зарегистрирован в CpuMetricsController");
        }

        /// <summary>
        /// Получение CPU метрик в заданный промежуток времени.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetrics([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetrics : fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(fromTime, toTime);

            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
                     
            return Ok(response);
        }

        /// <summary>
        /// Получение CPU метрик за все время учета.
        /// </summary>
        /// <returns>Список всех метрик.</returns>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            _logger.LogDebug("GetAll : без параметров");

            var metrics = _repository.GetAll();

            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}

