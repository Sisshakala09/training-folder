using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvancedLibrary.Models;
using AdvancedLibrary.Repositories;

namespace AdvancedLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IUnitOfWork uow, ILogger<BooksController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        // MVC page shell
        public IActionResult Index() => View();

        // AJAX: paged list as partial html
        [HttpGet]
        public async Task<IActionResult> List(string? search, string? sort, int page = 1, int pageSize = 10)
        {
            var (items, total) = await _uow.Books.GetPagedAsync(search, sort, page, pageSize);
            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            return PartialView("_BookTable", items);
        }

        // AJAX: GET form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookups();
            return PartialView("_BookForm", new Book());
        }

        // AJAX: POST create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookDto dto)
        {
            try
            {
                var book = new Book
                {
                    Title = dto.Title,
                    AuthorId = dto.AuthorId,
                    PublishedYear = dto.PublishedYear
                };

                await _uow.Books.AddAsync(book);

                // attach selected genres
                var genres = await _uow.Genres.GetAllAsync(g => dto.GenreIds.Contains(g.Id));
                foreach (var g in genres) book.Genres.Add(g);

                await _uow.SaveAsync();
                return Json(new { ok = true, message = "Book created." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create failed");
                Response.StatusCode = 500;
                return Json(new { ok = false, message = "Failed to create book." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _uow.Books.GetByIdAsync(id, b => b.Genres);
            if (book == null) return NotFound();
            await LoadLookups();
            var dto = BookDto.From(book);
            return PartialView("_BookForm", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] BookDto dto)
        {
            try
            {
                var book = await _uow.Books.GetByIdAsync(id, b => b.Genres);
                if (book == null) return NotFound();

                book.Title = dto.Title;
                book.AuthorId = dto.AuthorId;
                book.PublishedYear = dto.PublishedYear;

                // reset genres
                book.Genres.Clear();
                var genres = await _uow.Genres.GetAllAsync(g => dto.GenreIds.Contains(g.Id));
                foreach (var g in genres) book.Genres.Add(g);

                _uow.Books.Update(book);
                await _uow.SaveAsync();

                return Json(new { ok = true, message = "Book updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit failed");
                Response.StatusCode = 500;
                return Json(new { ok = false, message = "Failed to update book." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var book = await _uow.Books.GetByIdAsync(id);
                if (book == null) return NotFound();

                _uow.Books.Remove(book);
                await _uow.SaveAsync();

                return Json(new { ok = true, message = "Book deleted." });
            }
            catch (DbUpdateException dbex)
            {
                _logger.LogWarning(dbex, "Delete constraint");
                Response.StatusCode = 400;
                return Json(new { ok = false, message = "Cannot delete this book (in use)." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed");
                Response.StatusCode = 500;
                return Json(new { ok = false, message = "Failed to delete book." });
            }
        }

        private async Task LoadLookups()
        {
            ViewBag.Authors = await _uow.Authors.GetAllAsync(orderBy: q => q.OrderBy(a => a.Name));
            ViewBag.Genres = await _uow.Genres.GetAllAsync(orderBy: q => q.OrderBy(g => g.Name));
        }

        // DTO (keeps AJAX payload tidy)
        public class BookDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public int AuthorId { get; set; }
            public int? PublishedYear { get; set; }
            public List<int> GenreIds { get; set; } = new();

            public static BookDto From(Book b) => new()
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                PublishedYear = b.PublishedYear,
                GenreIds = b.Genres.Select(g => g.Id).ToList()
            };
        }
    }
}
