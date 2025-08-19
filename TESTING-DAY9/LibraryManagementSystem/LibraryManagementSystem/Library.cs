using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class Library
    {
        public List<Book> Books { get; } = new List<Book>();

        public void AddBook(Book book)
        {
            Books.Add(book);
        }
    }
}
