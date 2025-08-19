using System.ComponentModel.DataAnnotations.Schema;

namespace MVCExample.Models
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
