using MetricsAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
{
    public interface INetworkMetricsRepository : IRepository<NetworkMetric>
    {

    }

    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private readonly SQLiteConnection connection;

        public NetworkMetricsRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public void Create(NetworkMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "INSERT INTO networkmetrics(value, time) VALUES(@value, @time)"
            };

            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "DELETE FROM networkmetrics WHERE id=@id"
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(NetworkMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE networkmetrics SET value = @value, time = @time WHERE id=@id;"
            };
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }

        public IList<NetworkMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM networkmetrics"
            };

            var returnList = new List<NetworkMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new NetworkMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = reader.GetInt64(0)
                    });
                }
            }

            return returnList;
        }

        public NetworkMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM networkmetrics WHERE id=@id"
            };

            using SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new NetworkMetric
                {
                    Id = reader.GetInt32(0),
                    Value = reader.GetInt32(0),
                    Time = reader.GetInt64(0)
                };
            }
            else
            {
                return null;
            }
        }

        public IList<NetworkMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM networkmetrics WHERE time <= @timeEnd AND time >= @timeStart"
            };
            cmd.Parameters.AddWithValue("@timeStart", timeStart.ToUnixTimeSeconds());
            cmd.Parameters.AddWithValue("@timeEnd", timeEnd.ToUnixTimeSeconds());

            var returnList = new List<NetworkMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new NetworkMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = reader.GetInt64(0) 
                    });
                }
            }

            return returnList;
        }
    }
}
