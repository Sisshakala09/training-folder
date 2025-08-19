//namespace MyRazorApp.Models
//{
//  public class Category
//{
//}
//}



using MyRazorApp.Models;

namespace RazorEFApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public List<Product>? Products { get; set; }
    }
}
