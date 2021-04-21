using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using MetricsManager.DAL.Interfaces;
using AutoMapper;
using MetricsManager.Client;
using Microsoft.Extensions.Logging;
using MetricsManager.DAL.Responses;
using MetricsManager.Client.Requests;
using MetricsManager.DAL.Models;

namespace MetricsManager.Jobs
{
	public class NetworkMetricsJob : IJob
	{
        private readonly IServiceProvider _provider;
        private readonly INetworkMetricsRepository _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMapper _mapper;
        private readonly IMetricsManagerClient _client;
        private readonly ILogger _logger;

        public NetworkMetricsJob(IServiceProvider provider, IMapper mapper, IMetricsManagerClient client, ILogger<NetworkMetricsJob> logger)
        {
            _provider = provider;
            using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                _repository = serviceScope.ServiceProvider.GetService<INetworkMetricsRepository>();
                _agentsRepository = serviceScope.ServiceProvider.GetService<IAgentsRepository>();
            }
            _mapper = mapper;
            _client = client;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var allAgentsInfo = _agentsRepository.GetAllAgentsInfo();

            foreach (var agentInfo in allAgentsInfo)
            {
                var last = _repository.GetLast(agentInfo.AgentId);
                var request = new GetAllNetWorkMetricsApiRequest()
                {
                    AgentUrl = agentInfo.AgentUrl,
                    FromTime = last,
                    ToTime = DateTimeOffset.UtcNow,
                };

                var response = _client.GetAllNetworkMetrics(request);

                if (response != null)
                {
                    if (response.Metrics[0].Time == last)
                    {
                        response.Metrics.RemoveAt(0);
                    }

                    foreach (var metric in response.Metrics)
                    {
                        var formatedMetric = _mapper.Map<NetworkMetric>(metric);
                        formatedMetric.AgentId = agentInfo.AgentId;
                        _repository.Create(formatedMetric);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
