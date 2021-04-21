using System.Collections.Generic;

namespace MetricsManager.DAL.Responses
{
	public class AllAgentsInfoResponse
	{
		public List<AgentInfoDto> Agents { get; set; }
	}
	
	public class AgentInfoDto
	{
		public int AgentId { get; set; }
		public string AgentUrl { get; set; }
	}
}
