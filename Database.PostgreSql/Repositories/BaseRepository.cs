using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public virtual IQueryable<T> Query()
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
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> set, bool saveChanges = true)
        {
            try
            {
                await _dbSet
                    .Where(filter)
                    .ExecuteUpdateAsync(set);

                if (saveChanges)
                {
                    return await SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
