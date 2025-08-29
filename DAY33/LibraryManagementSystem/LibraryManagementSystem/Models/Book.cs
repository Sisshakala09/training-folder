using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }
        

        // Foreign Key
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        // Many-to-Many: Book ↔ Genre
        public ICollection<Genre> Genres { get; set; }
    }
}
