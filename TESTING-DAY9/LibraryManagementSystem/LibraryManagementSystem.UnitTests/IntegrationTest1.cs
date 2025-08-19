using NUnit.Framework;
using LibraryManagementSystem;

namespace LibraryManagementSystem.UnitTests
{
    public class IntegrationTest1
    {
        [Test]
        public void AddBook_ShouldAddBookToList()
        {
            // Arrange
            var library = new Library();
            var book = new Book("Wings of Fire", "A.P.J Abdul Kalam", "ISBN123");

            // Act
            library.AddBook(book);

            // Assert
            Assert.That(library.Books.Contains(book));
        }
    }
}
