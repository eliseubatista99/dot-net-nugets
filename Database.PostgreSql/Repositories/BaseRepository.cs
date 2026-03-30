using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Database.PostgreSql.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> ReadQuery()
        {
            return _dbSet.AsNoTracking();
        }

        public virtual IQueryable<T> WriteQuery()
        {
            return _dbSet.AsQueryable();
        }


        public virtual async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> AddAsync(T entity, bool saveChanges = true)
        {
            try
            {
                await _dbSet.AddAsync(entity);

                if (saveChanges)
                {
                    return await SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in AddAsync: {e.Message}\nStackTrace: {e.StackTrace}");
                return false;
            }
        }

        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);

                if (saveChanges)
                {
                    return await SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in AddRangeAsync: {e.Message}\nStackTrace: {e.StackTrace}");

                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity, bool saveChanges = true)
        {
            try
            {
                _dbSet.Update(entity);

                if (saveChanges)
                {
                    return await SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in UpdateAsync: {e.Message}\nStackTrace: {e.StackTrace}");

                return false;
            }
        }

        public async Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, Action<T> updateAction, bool saveChanges = true)
        {
            try
            {
                var entities = await _dbSet.Where(filter).ToListAsync();

                if (!entities.Any())
                {
                    return false;
                }

                // Run update action for each entity
                foreach (var entity in entities)
                {
                    updateAction(entity);
                }

                if (saveChanges)
                {
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in UpdateAsync: {e.Message}\nStackTrace: {e.StackTrace}");
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool saveChanges = true)
        {
            try
            {
                await _dbSet.Where(predicate).ExecuteDeleteAsync();

                if (saveChanges)
                {
                    return await SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in DeleteAsync: {e.Message}\nStackTrace: {e.StackTrace}");

                return false;
            }
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in SaveChangesAsync: {e.Message}\nStackTrace: {e.StackTrace}");

                return false;
            }
        }
    }
}
