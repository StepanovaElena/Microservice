using AutoMapper;
using MetricsAgent.Models;
using MetricsAgent.Responses;
using System;

namespace MetricsAgent
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<CpuMetric, CpuMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
			CreateMap<DotNetMetric, DotNetMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
			CreateMap<HddMetric, HddMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
			CreateMap<NetworkMetric, NetworkMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
			CreateMap<RamMetric, RamMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
		}
	}
}
