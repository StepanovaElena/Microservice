using AutoMapper;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ILogger<AgentsController> _logger;
        private readonly IAgentsRepository _repository;
        private readonly IMapper _mapper;

        public AgentsController(ILogger<AgentsController> logger, IAgentsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _logger.LogInformation("NLog зарегистрирован в AgentsController");
        }

        [HttpGet("read")]
        public IActionResult ReadRegisteredAgents()
        {
            _logger.LogInformation("NLog вызван в ReadRegisteredAgents");

            var allAgentsInfo = _repository.GetAllAgentsInfo();

            var response = new AllAgentsInfoResponse();

            foreach (var agentInfo in allAgentsInfo)
            {
                response.Agents.Add(_mapper.Map<AgentInfoDto>(agentInfo));
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
        {
            _logger.LogInformation("NLog вызван в RegisterAgent");

            _repository.RegisterAgent(agentInfo);

            return Ok();
        }

        [HttpPut("enable/{agentId}")]
        public IActionResult EnableAgentById([FromRoute] int agentId)
        {
            _logger.LogInformation("NLog вызван в EnableAgentById");
            return Ok();
        }

        [HttpPut("disable/{agentId}")]
        public IActionResult DisableAgentById([FromRoute] int agentId)
        {
            _logger.LogInformation("NLog вызван в  DisableAgentById");
            return Ok();
        }
    }  
}
