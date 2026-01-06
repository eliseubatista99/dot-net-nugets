using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.PostgreSql.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query(); // To build queries
        Task<T?> GetByIdAsync(string id);
        Task<bool> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
