using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        public int AuthorId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Bio { get; set; }

        // One Author → Many Books
        public ICollection<Book> Books { get; set; }
    }
}
