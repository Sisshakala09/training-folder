using System.ComponentModel.DataAnnotations.Schema;
using RazorApp.Models;

namespace RazorApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("categ")]
        public int categId { get; set; }
        public Category cacteg { get; set; }
    }
}
