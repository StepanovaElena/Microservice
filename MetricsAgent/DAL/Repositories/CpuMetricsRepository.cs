﻿using Dapper;
using MetricsAgent.DAL.Handlers;
using MetricsAgent.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
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
                connection.Execute("INSERT INTO cpumetrics(value, time) VALUES(@value, @time)",
                  new { value = item.Value, time = item.Time });
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
                return connection.QuerySingle<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics WHERE id=@id", new { id = id });
            }
        }

        public IList<CpuMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {            
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<CpuMetric>("SELECT * FROM cpumetrics WHERE time <= @timeEnd AND time >= @timeStart",
                    new { timeStart = timeStart.ToUnixTimeSeconds(), timeEnd = timeEnd.ToUnixTimeSeconds() }).ToList();
            }
        }

        public CpuMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<CpuMetric>("SELECT * FROM cpumetrics WHERE time = (SELECT MAX(time) FROM cpumetrics)");
            }
        }
    }
}
