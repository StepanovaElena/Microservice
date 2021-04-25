using Dapper;
using MetricsAgent.DAL.Handlers;
using MetricsAgent.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
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
                connection.Execute("INSERT INTO networkmetrics(value, time) VALUES(@value, @time)",
                  new { value = item.Value, time = item.Time });
            }
        }

        public IList<NetworkMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics").ToList();
            }
        }

        public IList<NetworkMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE time <= @timeEnd AND time >= @timeStart",
                    new { timeStart = timeStart.ToUnixTimeSeconds(), timeEnd = timeEnd.ToUnixTimeSeconds() }).ToList();
            }
        }

        public NetworkMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<NetworkMetric>("SELECT * FROM networkmetrics WHERE time = (SELECT MAX(time) FROM networkmetrics)");
            }
        }
    }
}

