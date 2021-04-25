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
    public class RamMetricsRepository : IRamMetricsRepository
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public RamMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        }

        public void Create(RamMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO rammetrics(value, time, agentId) VALUES(@value, @time, @agentId)",
                  new { value = item.Value, time = item.Time, agentId = item.AgentId, });
            }
        }

        public IList<RamMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT Id, Time, Value FROM rammetrics").ToList();
            }
        }

        public RamMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<RamMetric>("SELECT Id, Time, Value FROM rammetrics WHERE id=@id", new { id });
            }
        }

        public IList<RamMetric> GetInTimePeriod(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT * FROM rammetrics WHERE agentId = @agentId AND time <= @timeEnd AND time >= @timeStart",
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
                return connection.QueryFirst<DateTimeOffset>("SELECT MAX(Time) FROM rammetrics WHERE agentId = @agentId",
                    new { agentId });
            }
        }

        public RamMetric GetInTimePeriodPercentile(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd, Percentile percentile)
        {
            // Nearest rank method.
            var metricsInTimePeriod = GetInTimePeriod(agentId, timeStart, timeEnd).OrderBy(m => m.Value).ToList();
            var percentileIndex = ((int)percentile * metricsInTimePeriod.Count / 100);

            return metricsInTimePeriod[percentileIndex - 1];
        }
    }
}
