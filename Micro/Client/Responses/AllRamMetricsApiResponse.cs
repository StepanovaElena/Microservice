using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client
{
    public class AllRamMetricsApiResponse
    {
        public List<RamMetricDto> Metrics { get; set; }
    }
}