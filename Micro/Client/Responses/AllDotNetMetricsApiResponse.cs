using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client
{
    public class AllDotNetMetricsApiResponse
    {
        public List<DotNetMetricDto> Metrics { get; set; }
    }
}