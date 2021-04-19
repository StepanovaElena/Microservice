using AutoMapper;
using MetricsAgent.Models;
using MetricsAgent.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<CpuMetric, DotNetMetricDto>();
			CreateMap<DotNetMetric, DotNetMetricDto>();
			CreateMap<HddMetric, HddMetricDto>();
			CreateMap<NetworkMetric, NetworkMetricDto>();
			CreateMap<RamMetric, RamMetricDto>();
		}
	}
}
