using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, StringLength(180)]
        public string Title { get; set; } = string.Empty;

        [Range(1000, 9999)]
        public int? PublishedYear { get; set; }

        // FK -> Author
        [Required]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        // M2M -> Genres
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
