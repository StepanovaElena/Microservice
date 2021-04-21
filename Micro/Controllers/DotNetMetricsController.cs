using AutoMapper;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Responses;
using MetricsManager.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/dotnet")]
    [ApiController]
    public class DotNetMetricsController : Controller
    {
        private readonly ILogger<DotNetMetricsController> _logger;
        private readonly IDotNetMetricsRepository _repository;
        private readonly IAgentsRepository _agentRepository;
        private readonly IMapper _mapper;

        public DotNetMetricsController(
            ILogger<DotNetMetricsController> logger,
            IDotNetMetricsRepository repository,
            IAgentsRepository agentRepository,
            IMapper mapper)
        {
            _logger = logger;
            _logger.LogDebug("NLog зарегистрирован в DotNetMetricsController");

            _repository = repository;
            _mapper = mapper;
            _agentRepository = agentRepository;
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");

            var metrics = _repository.GetInTimePeriod(agentId, fromTime, toTime);

            var response = new AllDotNetMetricsResponse();

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }

            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAgent : agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");

            var metric = _repository.GetInTimePeriodPercentile(agentId, fromTime, toTime, percentile);

            return Ok(_mapper.Map<DotNetMetricDto>(metric));
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogDebug($"GetMetricsFromAllCluster : fromTime = {fromTime}; toTime = {toTime}");

            var agents = _agentRepository.GetAllAgentsInfo();

            var response = new AllDotNetMetricsResponse();

            foreach (var agent in agents)
            {
                var metrics = _repository.GetInTimePeriod(agent.AgentId, fromTime, toTime); ;

                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
                }
            }

            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime, [FromRoute] Percentile percentile)
        {
            _logger.LogDebug($"GetMetricsByPercentileFromAllCluster : fromTime = {fromTime}; toTime = {toTime}; percentile = {percentile}");

            var agents = _agentRepository.GetAllAgentsInfo();

            var response = new AllDotNetMetricsResponse();

            foreach (var agent in agents)
            {
                var metric = _repository.GetInTimePeriodPercentile(agent.AgentId, fromTime, toTime, percentile);
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
