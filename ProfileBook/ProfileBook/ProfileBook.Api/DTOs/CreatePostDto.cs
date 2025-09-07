using Microsoft.AspNetCore.Http;

namespace ProfileBook.Api.Models.Dto {
  public class CreatePostDto {
    public string Content { get; set; } = "";
    public IFormFile? Image { get; set; }
  }
}
