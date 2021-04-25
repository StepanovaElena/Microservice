using MetricsManager.DAL.Models;
using System.Collections.Generic;

namespace MetricsManager.DAL.Interfaces
{
	public interface IAgentsRepository
	{
		List<AgentInfo> GetAllAgentsInfo();
		
		AgentInfo GetAgentInfoById(int agentId);
		
		void RegisterAgent(AgentInfo agentInfo);
	}
}
