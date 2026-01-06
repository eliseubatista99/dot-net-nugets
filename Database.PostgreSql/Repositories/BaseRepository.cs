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

        public virtual async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public Task<int> UpdateAsync(
            Expression<Func<T, bool>> filter,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> set)
        {
            return _dbSet
                .Where(filter)
                .ExecuteUpdateAsync(set);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet
                .Where(predicate)
                .ExecuteDeleteAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
