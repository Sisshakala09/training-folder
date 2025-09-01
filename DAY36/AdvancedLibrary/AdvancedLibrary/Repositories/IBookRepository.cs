using AdvancedLibrary.Models;

namespace AdvancedLibrary.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<(IEnumerable<Book> Items, int Total)> GetPagedAsync(string? search, string? sort,
            int page, int pageSize);
    }
}
