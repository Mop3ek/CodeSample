using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Code.Sample
{
    /// <summary>
    /// Имитация контекста
    /// </summary>
    public class SampleContext: DbContext
    {
        /// <summary>
        /// Откат изменений сущности
        /// </summary>
        /// <param name="entity"></param>
        public void DiscardEntityChanges(object entity)
        {
            var changedEntries = ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        /// <summary>
        /// Обернутое сохранение контекста
        /// (что-бы при ошибке не падало все приложение)
        /// </summary>
        /// <returns>если успешно - возвращает true </returns>
        public async Task<bool> SafetySaveChanges()
        {
            bool res = false;
            try
            {
                await SaveChangesAsync();
                res = true;
            }
            catch (Exception e)
            {
                string msg = e.Message;

                msg = $"Исключение:{e.Message}";
                
                Exception innerException = e.InnerException;
                while (innerException != null)
                {
                    msg += $"\r\nВнутреннее исключение: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                Logger.AddErrorEx(msg, e);
                throw;
            }
            return res;
        }
    }
}