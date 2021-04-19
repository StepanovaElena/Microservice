using MetricsAgent.DAL;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using MetricsAgent.Models;

namespace MetricsAgent.Jobs
{
	public class RamMetricsJob : IJob
	{
		private readonly IServiceProvider _provider;
		private readonly IRamMetricsRepository _repository;
		private readonly PerformanceCounter _counter;

		public RamMetricsJob(IServiceProvider provider)
		{
			_provider = provider;
			using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				_repository = serviceScope.ServiceProvider.GetService<IRamMetricsRepository>();
			}
			_counter = new PerformanceCounter("Memory", "Available MBytes");
		}

		public Task Execute(IJobExecutionContext context)
		{
			int value = Convert.ToInt32(_counter.NextValue());
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			_repository.Create(new RamMetric { Time = time, Value = value });

			return Task.CompletedTask;
		}
	}
}
