using Dapper;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using MetricsManager.Enums;
using System.Data.SQLite;
using MetricsManager.DAL.Handlers;

namespace MetricsManager.DAL.Repositories
{
    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public CpuMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        }

        public void Create(CpuMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO cpumetrics(value, time, agentId) VALUES(@value, @time, @agentId)",
                  new { value = item.Value, time = item.Time, agentId = item.AgentId, });
            }
        }

        public IList<CpuMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics").ToList();
            }
        }

        public CpuMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics WHERE id=@id", new { id });
            }
        }

        public IList<CpuMetric> GetInTimePeriod(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<CpuMetric>("SELECT * FROM cpumetrics WHERE agentId == @agentId AND time <= @timeEnd AND time >= @timeStart",
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
                return connection.QueryFirst<DateTimeOffset>("SELECT MAX(Time) FROM cpumetrics WHERE agentId == @agentId",
                    new { agentId });
            }
        }

        public CpuMetric GetInTimePeriodPercentile(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd, Percentile percentile)
        {
            // Nearest rank method.
            var metricsInTimePeriod = GetInTimePeriod(agentId, timeStart, timeEnd).OrderBy(m => m.Value).ToList();
            var percentileIndex = ((int)percentile * metricsInTimePeriod.Count / 100);

            return metricsInTimePeriod[percentileIndex - 1];
        }
    }
}
