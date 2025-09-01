using Microsoft.EntityFrameworkCore;
using AdvancedLibrary.Data;
using AdvancedLibrary.Models;

namespace AdvancedLibrary.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<(IEnumerable<Book> Items, int Total)> GetPagedAsync(
            string? search, string? sort, int page, int pageSize)
        {
            var q = _ctx.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(b => b.Title.Contains(search) || b.Author!.Name.Contains(search));

            q = sort switch
            {
                "title_desc" => q.OrderByDescending(b => b.Title),
                "year" => q.OrderBy(b => b.PublishedYear),
                "year_desc" => q.OrderByDescending(b => b.PublishedYear),
                "author" => q.OrderBy(b => b.Author!.Name),
                "author_desc" => q.OrderByDescending(b => b.Author!.Name),
                _ => q.OrderBy(b => b.Title)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}
