using MetricsAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
{    
    public interface IHddMetricsRepository : IRepository<HddMetric>
    {

    }

    public class HddMetricsRepository : IHddMetricsRepository
    {        
        private readonly SQLiteConnection connection;

        public HddMetricsRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public void Create(HddMetric item)
        {           
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "INSERT INTO hddmetrics(value, time) VALUES(@value, @time)"
            };

            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "DELETE FROM hddmetrics WHERE id=@id"
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(HddMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE hddmetrics SET value = @value, time = @time WHERE id=@id;"
            };

            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public IList<HddMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM hddmetrics"
            };

            var returnList = new List<HddMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new HddMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(0))
                    });
                }
            }

            return returnList;
        }

        public HddMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM hddmetrics WHERE id=@id"
            };

            using SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new HddMetric
                {
                    Id = reader.GetInt32(0),
                    Value = reader.GetInt32(0),
                    Time = TimeSpan.FromSeconds(reader.GetInt32(0))
                };
            }
            else
            {
                return null;
            }
        }

        public IList<HddMetric> GetInTimePeriod(TimeSpan timeStart, TimeSpan timeEnd)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM hddmetrics WHERE time <= @timeEnd AND time >= @timeStart"
            };
            cmd.Parameters.AddWithValue("@timeStart", timeStart.TotalSeconds);
            cmd.Parameters.AddWithValue("@timeEnd", timeEnd.TotalSeconds);

            var returnList = new List<HddMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new HddMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(0))
                    });
                }
            }

            return returnList;
        }
    }
}
