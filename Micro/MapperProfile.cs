using AutoMapper;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Responses;
using System;

namespace MetricsManager
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AgentInfo, AgentInfoDto>();
            CreateMap<CpuMetric, CpuMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
            CreateMap<DotNetMetric, DotNetMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
            CreateMap<HddMetric, HddMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
            CreateMap<NetworkMetric, NetworkMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
            CreateMap<RamMetric, RamMetricDto>().ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)));
        }
    }
}
