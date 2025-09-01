using System.Linq.Expressions;

namespace AdvancedLibrary.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null, int? take = null,
            params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    }
}
