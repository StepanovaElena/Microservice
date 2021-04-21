using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client
{
    public class AllCpuMetricsApiResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
}