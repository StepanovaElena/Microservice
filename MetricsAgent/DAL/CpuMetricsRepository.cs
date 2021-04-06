using MetricsAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.DAL
{
    // маркировочный интерфейс
    // необходим, чтобы проверить работу репозитория на тесте-заглушке
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
    {

    }

    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        // наше соединение с базой данных
        private readonly SQLiteConnection connection;

        // инжектируем соединение с базой данных в наш репозиторий через конструктор
        public CpuMetricsRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public void Create(CpuMetric item)
        {
            // создаем команду
            using var cmd = new SQLiteCommand(connection)
            {
                // прописываем в команду SQL запрос на вставку данных
                CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)"
            };

            // добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);

            // в таблице будем хранить время в секундах, потому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time);
            // подготовка команды к выполнению
            cmd.Prepare();

            // выполнение команды
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                // прописываем в команду SQL запрос на удаление данных
                CommandText = "DELETE FROM cpumetrics WHERE id=@id"
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(CpuMetric item)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                // прописываем в команду SQL запрос на обновление данных
                CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id=@id;"
            };
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }

        public IList<CpuMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(connection)
            {
                // прописываем в команду SQL запрос на получение всех данных из таблицы
                CommandText = "SELECT * FROM cpumetrics"
            };

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // пока есть что читать -- читаем
                while (reader.Read())
                {
                    // добавляем объект в список возврата
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        // налету преобразуем прочитанные секунды в метку времени
                        Time = reader.GetInt64(0)
                    });
                }
            }

            return returnList;
        }

        public CpuMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM cpumetrics WHERE id=@id"
            };

            using SQLiteDataReader reader = cmd.ExecuteReader();
            // если удалось что то прочитать
            if (reader.Read())
            {
                // возвращаем прочитанное
                return new CpuMetric
                {
                    Id = reader.GetInt32(0),
                    Value = reader.GetInt32(0),
                    Time = reader.GetInt64(0)
                };
            }
            else
            {
                // не нашлось запись по идентификатору, не делаем ничего
                return null;
            }
        }

        public IList<CpuMetric> GetInTimePeriod(DateTimeOffset timeStart, DateTimeOffset timeEnd)
        {
            using var cmd = new SQLiteCommand(connection)
            {
                // прописываем в команду SQL запрос на получение всех данных из таблицы
                CommandText = "SELECT * FROM cpumetrics WHERE time <= @timeEnd AND time >= @timeStart"
            };
            cmd.Parameters.AddWithValue("@timeStart", timeStart.ToUnixTimeSeconds());
            cmd.Parameters.AddWithValue("@timeEnd", timeEnd.ToUnixTimeSeconds());

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // пока есть что читать -- читаем
                while (reader.Read())
                {
                    // добавляем объект в список возврата
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(0),
                        // налету преобразуем прочитанные секунды в метку времени
                        Time = reader.GetInt64(0)
                    });
                }
            }

            return returnList;
        }
    }
}
