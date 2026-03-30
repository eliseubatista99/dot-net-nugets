using Microsoft.EntityFrameworkCore;

namespace Database.PostgreSql.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<(List<T> Data, bool HasMorePages)> ExecutePaginatedRead<T>(
            this IQueryable<T> query,
            int? page,
            int? pageSize)
        {
            var hasMorePages = false;
            List<T> result;

            if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
            {
                int skip = (page.Value - 1) * pageSize.Value;

                result = await query
                    .Skip(skip)
                    // Take one more to know if there is another page
                    .Take(pageSize.Value + 1)
                    .ToListAsync();

                if (result.Count > pageSize.Value)
                {
                    hasMorePages = true;

                    // Takes off the extra final entry
                    result = result.Take(pageSize.Value).ToList();
                }
            }
            else
            {
                result = await query.ToListAsync();
            }

            return (result, hasMorePages);
        }
    }
}
