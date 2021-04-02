using MetricsAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
{
	public interface IDotNetMetricsRepository : IRepository<DotNetMetric>
	{
	}

	public class DotNetMetricsRepository : IDotNetMetricsRepository
	{
		private readonly SQLiteConnection connection;

		public DotNetMetricsRepository(SQLiteConnection connection)
		{
			this.connection = connection;
		}

        public void Create(DotNetMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "INSERT INTO dotnetmetrics(value, time) VALUES(@value, @time)"
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
                CommandText = "DELETE FROM dotnetmetrics WHERE id=@id"
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(DotNetMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE dotnetmetrics SET value = @value, time = @time WHERE id=@id;"
            };

            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public IList<DotNetMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM dotnetmetrics"
            };

            var returnList = new List<DotNetMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new DotNetMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(0))
                    });
                }
            }

            return returnList;
        }

        public DotNetMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM dotnetmetrics WHERE id=@id"
            };

            using SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new DotNetMetric
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

        public IList<DotNetMetric> GetInTimePeriod(TimeSpan timeStart, TimeSpan timeEnd)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM dotnetmetrics WHERE time <= @timeEnd AND time >= @timeStart"
            };
            cmd.Parameters.AddWithValue("@timeStart", timeStart.TotalSeconds);
            cmd.Parameters.AddWithValue("@timeEnd", timeEnd.TotalSeconds);

            var returnList = new List<DotNetMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new DotNetMetric
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
