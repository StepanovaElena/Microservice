using MetricsAgent.DAL;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace MetricsAgent.Jobs
{
    public class CpuMetricsJob: IJob
    {           
        private readonly IServiceProvider _provider;
        private readonly ICpuMetricsRepository _repository;        
        private readonly PerformanceCounter _counter;

        public CpuMetricsJob(IServiceProvider provider)
        {
            _provider = provider;
            using (var serviceScope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                _repository = serviceScope.ServiceProvider.GetService<ICpuMetricsRepository>();
            }            
            _counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public Task Execute(IJobExecutionContext context)
        {            
            var cpuUsageInPercents = Convert.ToInt32(_counter.NextValue());
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _repository.Create(new Models.CpuMetric { Time = time, Value = cpuUsageInPercents });

            return Task.CompletedTask;
        }
    }
}
