using Dapper;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL.Repositories
{
    public class AgentsRepository : IAgentsRepository
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public AgentInfo GetAgentInfoById(int agentId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<AgentInfo>("SELECT * FROM agents WHERE agentId = @agentId",
                new { agentId });
            }
        }

        public List<AgentInfo> GetAllAgentsInfo()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<AgentInfo>("SELECT * FROM agents").ToList();
            }
        }

        public void RegisterAgent(AgentInfo agentInfo)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO agents(agentId, agentUrl) VALUES(@agentId, @agentUri)",
                  new { agentId = agentInfo.AgentId, agentUri = agentInfo.AgentUrl });
            }
        }
    }
}
