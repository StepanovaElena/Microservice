using MetricsAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
{
    public interface IRamMetricsRepository : IRepository<RamMetric>
    {

    }

    public class RamMetricsRepository : IRamMetricsRepository
    {
        private readonly SQLiteConnection connection;

        public RamMetricsRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public void Create(RamMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)"
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
                CommandText = "DELETE FROM cpumetrics WHERE id=@id"
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(RamMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id=@id;"
            };
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }

        public IList<RamMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM cpumetrics"
            };

            var returnList = new List<RamMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new RamMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = reader.GetInt64(0)
                    });
                }
            }

            return returnList;
        }

        public RamMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM cpumetrics WHERE id=@id"
            };

            using SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new RamMetric
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

        public IList<RamMetric> GetInTimePeriod(DateTimeOffset timeFrom, DateTimeOffset timeTo)
        {
            throw new NotImplementedException();
        }
    }
}
