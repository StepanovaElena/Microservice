using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();

        IList<T> GetInTimePeriod(DateTimeOffset timeFrom, DateTimeOffset timeTo);

        T GetById(int id);

        void Create(T item);

        void Update(T item);

        void Delete(int id);
    }
}
