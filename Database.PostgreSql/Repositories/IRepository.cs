using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Database.PostgreSql.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> ReadQuery();

        public IQueryable<T> WriteQuery();

        public Task<T?> GetByIdAsync(string id);

        public Task<bool> AddAsync(T entity, bool saveChanges = true);

        public Task<bool> UpdateAsync(T entity, bool saveChanges = true);

        public Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> set, bool saveChanges = true);

        public Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool saveChanges = true);

        public Task<bool> SaveChangesAsync();
    }
}
