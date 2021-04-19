using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MetricsAgent.Models;

namespace MetricsAgent.Jobs
{
    public class DotNetMetricsJob : IJob
    {        
        private readonly IServiceProvider _provider;
        private readonly IDotNetMetricsRepository _repository;        
        private readonly PerformanceCounter _counter;

        public DotNetMetricsJob(IServiceProvider provider)
        {
            _provider = provider;
            using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                _repository = serviceScope.ServiceProvider.GetService<IDotNetMetricsRepository>();
            }
            _counter = new PerformanceCounter(".NET CLR Memory", "# Bytes in all Heaps", "_Global_");
        }

        public Task Execute(IJobExecutionContext context)
        {            
            int value = Convert.ToInt32(_counter.NextValue());
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _repository.Create(new DotNetMetric { Time = time, Value = value });

            return Task.CompletedTask;
        }
    }
}
