using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Code.Sample
{
    public static class DataHelper
    {
        /// <summary>
        /// Получение репозитория по типу сущности
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <returns>репозиторий</returns>
        public static IRepository<T> GetRepository<T>() where T : class, IEntity, new()
        {
            return DataFactory.CreateUniOrDefaultRepository(typeof(T)) as IRepository<T>;
        }

        /// <summary>
        /// Получение полного списка сущностей
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <returns>коллекция сущностей</returns>
        public static List<T> GetAll<T>() where T : class, IEntity, new()
        {
            List<T> res = null;
            using (IRepository<T> repo = GetRepository<T>())
            {
                res = repo.GetAll()?.ToList();
            }
            if (res == null)
                res = new List<T>();
            return res;
        }

        /// <summary>
        /// Сохранение указанной сущности
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="entity">экземпляр сущности</param>
        /// <returns>результат сохранения (true-упешно)</returns>
        public static async Task<bool> Save<T>(T entity) where T : class, IEntity, new()
        {
            bool res = false;
            using (IRepository<T> repo = GetRepository<T>())
            {
                res = await repo.Save(entity);
            }
            return res;
        }

        /// <summary>
        /// Удаление указанной сущности
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="entity">экземпляр сущности</param>
        /// <returns>результат удаления (true-упешно)</returns>
        public static bool Delete<T>(T entity) where T : class, IEntity, new()
        {
            bool res = false;
            using (IRepository<T> repo = GetRepository<T>())
            {
                repo.Delete(entity.Id);
                res = true;
            }
            return res;
        }

        /// <summary>
        /// Сохранение набора сущьностей
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="entities">коллекция экземпляров сущности</param>
        /// <returns>результат сохранения (true-упешно)</returns>
        public static async Task<bool> SaveCollection<T>(IEnumerable<T> entities) where T : class, IEntity, new()
        {
            bool res = false;
            using (IRepository<T> repo = GetRepository<T>())
            {
                res = await repo.Save(entities);
            }
            return res;
        }

        /// <summary>
        /// Получение сущности по идентификатору
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="id">идентификатор сущности</param>
        /// <returns>екземпляр сущности</returns>
        public static async Task<T> GetItem<T>(Guid id)
            where T : class, IEntity, new()
        {
            T res = null;

            using (IRepository<T> repo = GetRepository<T>())
            {
                res = await repo.GetItem(id);
            }

            return res;
        }
    }
}