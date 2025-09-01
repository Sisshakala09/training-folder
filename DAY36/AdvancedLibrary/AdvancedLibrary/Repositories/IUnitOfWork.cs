using System.Threading.Tasks;

namespace AdvancedLibrary.Repositories
{
    public interface IUnitOfWork
    {
        IBookRepository Books { get; }
        IGenericRepository<AdvancedLibrary.Models.Author> Authors { get; }
        IGenericRepository<AdvancedLibrary.Models.Genre> Genres { get; }

        Task<int> SaveAsync();
    }
}
