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
                connection.Execute("INSERT INTO hddmetrics(value, time) VALUES(@value, @time)",
                  new { value = item.Value, time = item.Time });
            }
        }

        public IList<HddMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<HddMetric>("SELECT Id, Time, Value FROM hddmetrics").ToList();
            }
        }

        public IList<HddMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {            
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<HddMetric>("SELECT * FROM hddmetrics WHERE time <= @timeEnd AND time >= @timeStart",
                    new { timeStart = timeStart.ToUnixTimeSeconds(), timeEnd = timeEnd.ToUnixTimeSeconds() }).ToList();
            }
        }

        public HddMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<HddMetric>("SELECT * FROM hddmetrics WHERE time = (SELECT MAX(time) FROM hddmetrics)");
            }
        }
    }
}
