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
                connection.Execute("INSERT INTO dotnetmetrics(value, time) VALUES(@value, @time)",
                  new { value = item.Value, time = item.Time });
            }
        }

        public IList<DotNetMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics").ToList();
            }
        }

        public IList<DotNetMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT * FROM dotnetmetrics WHERE Time <= @timeEnd AND Time >= @timeStart",
                    new { timeStart = timeStart.ToUnixTimeSeconds(), timeEnd = timeEnd.ToUnixTimeSeconds() }).ToList();
            }
        }

        public DotNetMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<DotNetMetric>("SELECT * FROM dotnetmetrics WHERE Time = (SELECT MAX(time) FROM dotnetmetrics)");
            }
        }
    }
}
