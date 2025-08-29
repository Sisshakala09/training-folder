using Microsoft.AspNetCore.Mvc;
using BookstoreAdoNet.Data;
using BookstoreAdoNet.Models;

namespace BookstoreAdoNet.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookRepository _repo;

        public BooksController(BookRepository repo)
        {
            _repo = repo;
        }

        // 1. Show list of books
        public IActionResult Index()
        {
            var list = _repo.GetAllBooks();  // get all from DB
            return View(list);               // send to Views/Books/Index.cshtml
        }

        // 2. Show details of one book
        public IActionResult Details(int id)
        {
            var book = _repo.GetBookById(id);
            if (book == null) return NotFound();
            return View(book); // Views/Books/Details.cshtml
        }

        // 3. Show Create form
        public IActionResult Create()
        {
            return View(); // Views/Books/Create.cshtml
        }

        // 4. Save new book (from Create form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _repo.AddBook(book);   // Save in DB
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // 5. Show Edit form
        public IActionResult Edit(int id)
        {
            var book = _repo.GetBookById(id);
            if (book == null) return NotFound();
            return View(book); // Views/Books/Edit.cshtml
        }

        // 6. Save updated book (from Edit form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdateBook(book);  // Update DB
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // 7. Show Delete confirmation
        public IActionResult Delete(int id)
        {
            var book = _repo.GetBookById(id);
            if (book == null) return NotFound();
            return View(book); // Views/Books/Delete.cshtml
        }

        // 8. Delete confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.DeleteBook(id);  // Remove from DB
            return RedirectToAction(nameof(Index));
        }
    }
}
