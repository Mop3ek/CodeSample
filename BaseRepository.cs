using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Code.Sample
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity, new()
    {
        public abstract Task<bool> Save(IEnumerable<T> entities);
        public abstract Task<bool> Save(T entity);
        public abstract T Revert(T entity);
        public abstract Task Delete(Guid id);
        public abstract Task DeleteAll(IEnumerable<T> entities);
        public abstract Task<T> GetItem(Guid id);
        public abstract IQueryable<T> GetAll();
        public abstract IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        public abstract Task<T> GetItem(Expression<Func<T, bool>> predicate);
        public abstract IQueryable<T> GetIncludeAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includeProperties);
        public abstract Task<T> GetIncludeItem(Guid id, params Expression<Func<T, object>>[] includeProperties);
        public abstract Task<bool> Any(Expression<Func<T, bool>> criteria);
        public abstract void Dispose();
        
    }
}