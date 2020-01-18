using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Attendance.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long id);
        T GetById(params object[] keyValues);
        T GetById(string id);
        T GetById(Guid id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        void BulkSave(List<T> entities);
//        void ExecuteSP(long p1, int p2, int p3);
        void ExecuteSP(string procName, Dictionary<string, object> keyValuePairs);
    }
}
