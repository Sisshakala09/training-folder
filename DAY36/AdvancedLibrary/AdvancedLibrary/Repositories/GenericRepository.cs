using System.Collections.Generic;
using System.Linq.Expressions;
using AdvancedLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedLibrary.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _ctx;
        protected readonly DbSet<T> _set;

        public GenericRepository(AppDbContext ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set.AsQueryable();
            foreach (var inc in includes) query = query.Include(inc);
            // assumes integer PK property "Id"
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, "Id");
            var body = Expression.Equal(prop, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return await query.AsNoTracking().FirstOrDefaultAsync(lambda);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null, int? take = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set.AsQueryable();
            if (filter != null) query = query.Where(filter);
            foreach (var inc in includes) query = query.Include(inc);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(T entity) => await _set.AddAsync(entity);
        public void Update(T entity) => _set.Update(entity);
        public void Remove(T entity) => _set.Remove(entity);

        public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null) =>
            filter == null ? _set.CountAsync() : _set.CountAsync(filter);
    }
}
