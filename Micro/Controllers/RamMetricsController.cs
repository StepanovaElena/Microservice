using Microsoft.AspNetCore.Mvc;
using System;
using MetricsManager.Enums;
using Microsoft.Extensions.Logging;
using MetricsManager.DAL.Interfaces;
using AutoMapper;
using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]

    public class RamMetricsController : Controller
    {
        private readonly ILogger<RamMetricsController> _logger;
        private readonly IRamMetricsRepository _repository;
        private readonly IAgentsRepository _agentRepository;
        private readonly IMapper _mapper;

        public RamMetricsController(
            ILogger<RamMetricsController> logger,
            IRamMetricsRepository repository,
            IAgentsRepository agentRepository,
            IMapper mapper)
        {
            _logger = logger;
            _logger.LogDebug("NLog зарегистрирован в RamMetricsController");

            _repository = repository;
            _mapper = mapper;
            _agentRepository = agentRepository;
        }

        /// <summary>
        /// Получение RAM метрик в заданный промежуток времени от конкретного агента.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <param name="agentId">Идентификатор агента.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(agentId, fromTime, toTime);

            var response = new AllRamMetricsResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получение RAM метрик в заданный промежуток времени от конкретного агента c учетом значения процентиля.
        /// </summary>
        /// <param name="agentId">Идентификатор агента.</param>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <param name="percentile">Значение процентиля.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");

            var metric = _repository.GetInTimePeriodPercentile(agentId, fromTime, toTime, percentile);

            return Ok(_mapper.Map<RamMetricDto>(metric));
        }

        /// <summary>
        /// Получение RAM метрик в заданный промежуток времени от всех агентов.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAllCluster : fromTime = {fromTime}; toTime = {toTime}");

            var agents = _agentRepository.GetAllAgentsInfo();

            var response = new AllRamMetricsResponse() 
            { 
                Metrics = new List<RamMetricDto>() 
            };

            foreach (var agent in agents)
            {
                var metrics = _repository.GetInTimePeriod(agent.AgentId, fromTime, toTime); ;

                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Получение RAM метрик в заданный промежуток времени от всех агентов c учетом значения процентиля.
        /// </summary>
        /// <param name="fromTime">Временная метка начала выборки.</param>
        /// <param name="toTime">Временная метка окончания выборки.</param>
        /// <param name="agentId">Идентификатор агента.</param>
        /// <param name="percentile">Значение процентиля.</param>
        /// <returns>Список метрик в заданный интервал времени.</returns>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAllCluster : fromTime = {fromTime}; toTime = {toTime}; percentile = {percentile}");

            var agents = _agentRepository.GetAllAgentsInfo();

            var response = new AllRamMetricsResponse() 
            { 
                Metrics = new List<RamMetricDto>() 
            };

            foreach (var agent in agents)
            {
                var metric = _repository.GetInTimePeriodPercentile(agent.AgentId, fromTime, toTime, percentile);
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
