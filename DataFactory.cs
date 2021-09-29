using System;
using System.Linq;
using System.Reflection;

namespace Code.Sample
{
    /// <summary>
    /// Фабрика репозиториев
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// Создание репозитория
        /// </summary>
        /// <param name="T">тип сущности</param>
        /// <returns></returns>
        public static object CreateUniOrDefaultRepository(Type T)
        {
            //ищем специализированй репозиторий
            object repo = CreateRepository(T);

            //если нет - создаем универсальный
            if (repo == null)
                repo = CreateUniversalRepository(T);
            return repo;
        }

        /// <summary>
        /// Создает универсальный репозиторий по типу сущности
        /// </summary>
        /// <param name="T">тип сущьности</param>
        /// <returns>универсальный репозиторий</returns>
        private static object CreateUniversalRepository(Type T)
        {
            Type concreteType = typeof(UniversalRepository<>).MakeGenericType(T);

            if (concreteType != null)
                return Activator.CreateInstance(concreteType);
            return null;
        }

        /// <summary>
        /// Создает (если определен) специализированый репозиторий
        /// </summary>
        /// <param name="T">тип сущности</param>
        /// <returns></returns>
        private static object CreateRepository(Type T)
        {
            Type repType = typeof(IRepository<>).MakeGenericType(T);

            var ass = Assembly.GetExecutingAssembly();
            var types = ass.GetTypes()
                .Where(p => p.IsClass &&
                            !p.IsAbstract &&
                            repType.IsAssignableFrom(p));

            var concreteType = types.FirstOrDefault();
            if (concreteType != null)
                return Activator.CreateInstance(concreteType);
            return null;
        }
    }
}