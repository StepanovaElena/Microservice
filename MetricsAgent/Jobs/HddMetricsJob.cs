using MetricsAgent.DAL;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using MetricsAgent.Models;

namespace MetricsAgent.Jobs
{
	public class HddMetricsJob : IJob
	{
		private readonly IServiceProvider _provider;
		private readonly IHddMetricsRepository _repository;		
		private readonly PerformanceCounter _counter;

		public HddMetricsJob(IServiceProvider provider)
		{
			_provider = provider;
			using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				_repository = serviceScope.ServiceProvider.GetService<IHddMetricsRepository>();
			}
			_counter = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total");
		}

		public Task Execute(IJobExecutionContext context)
		{			
			int value = Convert.ToInt32(_counter.NextValue());			
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			_repository.Create(new HddMetric { Time = time, Value = value });

			return Task.CompletedTask;
		}
	}
}
