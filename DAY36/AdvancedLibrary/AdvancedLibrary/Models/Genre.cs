using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedLibrary.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
