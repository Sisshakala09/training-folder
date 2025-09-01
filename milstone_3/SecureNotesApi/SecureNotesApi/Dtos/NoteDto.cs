using System.ComponentModel.DataAnnotations;

namespace SecureNotesApi.Dtos
{
    public class NoteDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
