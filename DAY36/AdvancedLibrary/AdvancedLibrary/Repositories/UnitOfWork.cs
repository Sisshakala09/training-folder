using AdvancedLibrary.Data;
using AdvancedLibrary.Models;

namespace AdvancedLibrary.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _ctx;
        public IBookRepository Books { get; }
        public IGenericRepository<Author> Authors { get; }
        public IGenericRepository<Genre> Genres { get; }

        public UnitOfWork(AppDbContext ctx, IBookRepository books)
        {
            _ctx = ctx;
            Books = books;
            Authors = new GenericRepository<Author>(_ctx);
            Genres = new GenericRepository<Genre>(_ctx);
        }

        public Task<int> SaveAsync() => _ctx.SaveChangesAsync();
    }
}
