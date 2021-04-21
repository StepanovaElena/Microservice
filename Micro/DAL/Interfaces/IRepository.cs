using MetricsManager.Enums;
using System;
using System.Collections.Generic;

namespace MetricsManager.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Create(T item);

        IList<T> GetAll();

        T GetById(int id);

        IList<T> GetInTimePeriod(int agentId, DateTimeOffset timeFrom, DateTimeOffset timeTo);

        T GetInTimePeriodPercentile(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile);

        DateTimeOffset GetLast(int agentId);
    }
}
