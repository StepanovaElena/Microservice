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
    public class DotNetMetricsRepository : IDotNetMetricsRepository
	{
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public DotNetMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        }

        public void Create(DotNetMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO dotnetmetrics(value, time, agentId) VALUES(@value, @time, @agentId)",
                  new { value = item.Value, time = item.Time, agentId = item.AgentId, });
            }
        }

        public IList<DotNetMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics").ToList();
            }
        }
        public DotNetMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics WHERE id=@id", new { id });
            }
        }

        public IList<DotNetMetric> GetInTimePeriod(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT * FROM dotnetmetrics WHERE agentId = @agentId AND time <= @timeEnd AND time >= @timeStart",
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
                return connection.QueryFirst<DateTimeOffset>("SELECT MAX(Time) FROM dotnetmetrics WHERE agentId = @agentId",
                    new { agentId });
            }
        }

        public DotNetMetric GetInTimePeriodPercentile(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd, Percentile percentile)
        {
            // Nearest rank method.
            var metricsInTimePeriod = GetInTimePeriod(agentId, timeStart, timeEnd).OrderBy(m => m.Value).ToList();
            var percentileIndex = ((int)percentile * metricsInTimePeriod.Count / 100);

            return metricsInTimePeriod[percentileIndex - 1];
        }
    }
}
