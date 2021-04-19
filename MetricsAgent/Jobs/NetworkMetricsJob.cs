using MetricsAgent.DAL;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using MetricsAgent.Models;
using System.Collections.Generic;

namespace MetricsAgent.Jobs
{
	public class NetworkMetricsJob : IJob
	{		
		private readonly IServiceProvider _provider;
		private readonly INetworkMetricsRepository _repository;
		private readonly List<PerformanceCounter> _counters = new List<PerformanceCounter>();


		public NetworkMetricsJob(IServiceProvider provider)
		{
			_provider = provider;
			using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				_repository = serviceScope.ServiceProvider.GetService<INetworkMetricsRepository>();
			}

			PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
			string[] instancename = category.GetInstanceNames();

			foreach (string name in instancename)
			{
				_counters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
			}
		}

		public Task Execute(IJobExecutionContext context)
		{
			var value = 0;
			
			foreach (var counter in _counters)
			{
				value += Convert.ToInt32(counter.NextValue());
			}

			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			_repository.Create(new NetworkMetric { Time = time, Value = value });

			return Task.CompletedTask;
		}
	}
}
