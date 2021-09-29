using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Code.Sample
{
    public interface IRepository<T> : IDisposable where T : class, IEntity, IDisposable, new()
    {
        Task<bool> Save(IEnumerable<T> entities);
        Task<bool> Save(T entity);
        T Revert(T entity);
        Task Delete(Guid id);
        Task DeleteAll(IEnumerable<T> entities);
        Task<T> GetItem(Guid id);
        Task<T> GetItem(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        Task<T> GetIncludeItem(Guid id, params Expression<Func<T, object>>[] includeProperties);
        Task<bool> Any(Expression<Func<T, bool>> criteria);
    }
}