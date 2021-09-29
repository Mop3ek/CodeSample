using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Code.Sample
{
    /// <summary>
    /// Универсальный репозиторий.
    /// Специализорованый репозиторий м.б унаследован от универсального
    /// с перегрузкой или добавлением необходимого метода репозидория
    /// </summary>
    /// <typeparam name="T">тип сущности контекста</typeparam>
    public class UniversalRepository<T> : BaseRepository<T> where T : class, IEntity, new()
    {
        public SampleContext SampleContext { get; set; }


        public override async Task<bool> Save(IEnumerable<T> entities)
        {
            bool _res = false;
            try
            {
                T[] _items = entities.ToArray();
                SampleContext.Set<T>().AddOrUpdate(_items);
                _res = await SampleContext.SafetySaveChanges();
                if (_res)
                {
                    foreach (var entity in entities)
                    {
                        entity.Changed = false;
                    }
                }
                else
                {
                    SampleContext.DiscardEntityChanges(_items);
                }
            }
            catch (Exception e)
            {
                Logger.AddErrorMsgEx($"{this}.Save", e);
            }

            return _res;
        }

        public override async Task<bool> Save(T entity)
        {
            bool _res = false;
            try
            {
                SampleContext.Set<T>().AddOrUpdate(entity);
                _res = await SampleContext.SafetySaveChanges();
                if (_res)
                {
                    entity.Changed = false;
                }
            }
            catch (Exception e)
            {
                Logger.AddErrorMsgEx($"{this}.Save", e);
            }
            return _res;
        }

        public override T Revert(T entity)
        {
            T res = entity;
            try
            {
                SampleContext.DiscardEntityChanges(entity);
                res.Changed = false;
            }
            catch (Exception e)
            {
                Logger.AddErrorEx("UniversalRepository.Revert", e);
            }
            return res;
        }

        public override async Task Delete(Guid id)
        {
            T _item =  await SampleContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            if (_item != null)
            {
                var _ent = SampleContext.Entry(_item);
                SampleContext.Set<T>().Remove(_item);
                await SampleContext.SafetySaveChanges();
            }
        }

        public override async Task DeleteAll(IEnumerable<T> entities)
        {
            foreach (var p in SampleContext.Set<T>())
            {
                SampleContext.Entry(p).State = EntityState.Deleted;
            }
            await SampleContext.SafetySaveChanges();
        }

        public override async Task<T> GetItem(Guid id)
        {
            T _item = null;
            try
            {
                _item = await SampleContext.Set<T>()?.FirstOrDefaultAsync(x => x.Id == id);
                if (_item != null)
                    _item.Changed = false;
            }
            catch (Exception e)
            {
                Logger.AddErrorMsgEx($"{this}.GetItem", e);
            }
            return _item;
        }

        public override async Task<T> GetItem(Expression<Func<T, bool>> predicate)
        {
            T _item = null;
            try
            {
                _item = await SampleContext.Set<T>()?.FirstOrDefaultAsync(predicate);

                if (_item != null)
                    _item.Changed = false;
            }
            catch (Exception e)
            {
                Logger.AddErrorMsgEx($"{this}.GetItem", e);
            }
            return _item;
        }

        public override async Task<T> GetIncludeItem(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            T _item = null;
            try
            {
                _item = GetWithIncludeCriteria(x => x.Id == id, includeProperties).FirstOrDefault();
                if (_item != null)
                    _item.Changed = false;
            }
            catch (Exception e)
            {
                Logger.AddErrorMsgEx($"{this}.GetItem", e);
            }
            return _item;
        }

        public override IQueryable<T> GetAll()
        {
            return SampleContext.Set<T>();
        }

        public override IQueryable<T> GetAll(Expression<Func<T, bool>> criteria)
        {
            return SampleContext.Set<T>().Where(criteria);
        }

        public override IQueryable<T> GetIncludeAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetWithIncludeCriteria(criteria, includeProperties);
        }

        private IQueryable<T> GetWithIncludeCriteria(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetSourceItemsWithInclude(includeProperties);
            return query.Where(predicate);
        }

        private IQueryable<T> GetSourceItemsWithInclude(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = SampleContext.Set<T>();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public override async Task<bool> Any(Expression<Func<T, bool>> criteria)
        {
            bool res = false;
            IQueryable<T> query = SampleContext.Set<T>();
            return await query.AnyAsync(criteria);
        }


        public override void Dispose()
        {
            if (SampleContext != null)
                SampleContext.Dispose();
        }


    }
}