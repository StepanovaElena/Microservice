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
    public class HddMetricsRepository : IHddMetricsRepository
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public HddMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        }

        public void Create(HddMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO hddmetrics(value, time, agentId) VALUES(@value, @time, @agentId)",
                  new { value = item.Value, time = item.Time, agentId = item.AgentId, });
            }
        }

        public IList<HddMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<HddMetric>("SELECT Id, Time, Value FROM hddmetrics").ToList();
            }
        }
        public HddMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<HddMetric>("SELECT Id, Time, Value FROM hddmetrics WHERE id=@id", new { id });
            }
        }

        public IList<HddMetric> GetInTimePeriod(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<HddMetric>("SELECT * FROM hddmetrics WHERE agentId = @agentId AND time <= @timeEnd AND time >= @timeStart",
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
            DateTimeOffset lastTime = DateTimeOffset.FromUnixTimeSeconds(0);

            using var connection = new SQLiteConnection(ConnectionString);
            try
            {
                lastTime = connection.QueryFirst<DateTimeOffset>("SELECT MAX(Time) FROM hddmetrics WHERE agentId = @agentId",
                 new { agentId });
            }
            catch (Exception)
            {

            }

            return lastTime;
        }

        public HddMetric GetInTimePeriodPercentile(int agentId, DateTimeOffset timeStart, DateTimeOffset timeEnd, Percentile percentile)
        {
            // Nearest rank method.
            var metricsInTimePeriod = GetInTimePeriod(agentId, timeStart, timeEnd).OrderBy(m => m.Value).ToList();
            var percentileIndex = ((int)percentile * metricsInTimePeriod.Count / 100);

            return metricsInTimePeriod[percentileIndex - 1];
        }
    }
}
