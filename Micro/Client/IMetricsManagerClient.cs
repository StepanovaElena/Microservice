using MetricsManager.Client.Requests;
using MetricsManager.Client.Responses;

namespace MetricsManager.Client
{
    public interface IMetricsManagerClient
    {
        AllRamMetricsApiResponse GetAllRamMetrics(GetAllRamMetricsApiRequest request);

        AllHddMetricsApiResponse GetAllHddMetrics(GetAllHddMetricsApiRequest request);

        AllDotNetMetricsApiResponse GetDonNetMetrics(GetAllDotNetMetricsApiRequest request);

        AllCpuMetricsApiResponse GetCpuMetrics(GetAllCpuMetricsApiRequest request);

        AllNetWorkMetricsApiResponse GetAllNetworkMetrics(GetAllNetWorkMetricsApiRequest request);        
    }

}
