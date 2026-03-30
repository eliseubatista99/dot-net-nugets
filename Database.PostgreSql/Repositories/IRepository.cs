using System.Linq.Expressions;

namespace Database.PostgreSql.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> ReadQuery();

        public IQueryable<T> WriteQuery();

        public Task<T?> GetByIdAsync(string id);

        public Task<bool> AddAsync(T entity, bool saveChanges = true);

        public Task<bool> AddRangeAsync(IEnumerable<T> entities, bool saveChanges = true);

        public Task<bool> UpdateAsync(T entity, bool saveChanges = true);

        public Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, Action<T> updateAction, bool saveChanges = true);

        public Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool saveChanges = true);

        public Task<bool> SaveChangesAsync();
    }
}
