using System;
using System.Collections.Generic;

namespace MetricsAgent.DAL
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();

        IList<T> GetInTimePeriod(DateTimeOffset timeFrom, DateTimeOffset timeTo);

        T GetById(int id);

        T GetLast();

        void Create(T item);
    }
}
