using MetricsManager.DAL.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client.Responses
{
    public class AllNetWorkMetricsApiResponse
    {
        public List<NetworkMetricDto> Metrics { get; set; }
    }
}
