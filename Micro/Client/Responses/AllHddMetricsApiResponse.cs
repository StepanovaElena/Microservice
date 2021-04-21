using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client
{
    public class AllHddMetricsApiResponse
    {
        public List<HddMetricDto> Metrics { get; set; }
    }
}