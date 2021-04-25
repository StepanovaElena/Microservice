using Dapper;
using MetricsManager.DAL.Handlers;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.Enums;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL.Repositories
{
    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public NetworkMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        }

        public void Create(NetworkMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO cpumetrics(value, time, agentId) VALUES(@value, @time, @agentId)",
                  new { value = item.Value, time = item.Time, agentId = item.AgentId, });
            }
        }

        public IList<NetworkMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics").ToList();
            }
        }
        public NetworkMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics WHERE id=@id", new { id });
            }
        }

        public IList<NetworkMetric> GetInTimePeriod(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE agentId = @agentId AND time <= @timeEnd AND time >= @timeStart",
                    new
                    {
                        timeStart = timeStart.ToUnixTimeSeconds(),
                        timeEnd = timeEnd.ToUnixTimeSeconds(),
                        agentId
                    }).ToList();
            }
        }

        public DateTimeOffset GetLast(int agentId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QueryFirst<DateTimeOffset>("SELECT MAX(Time) FROM networkmetrics WHERE agentId = @agentId",
                    new { agentId });
            }
        }

        public NetworkMetric GetInTimePeriodPercentile(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd, Percentile percentile)
        {
            // Nearest rank method.
            var metricsInTimePeriod = GetInTimePeriod(agentId, timeStart, timeEnd).OrderBy(m => m.Value).ToList();
            var percentileIndex = ((int)percentile * metricsInTimePeriod.Count / 100);

            return metricsInTimePeriod[percentileIndex - 1];
        }
    }
}

