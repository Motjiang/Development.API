using System.ComponentModel.DataAnnotations;

namespace Development.API.Models.Domain
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string UrlHandle { get; set; }
        public string Status { get; set; } 

        public ICollection<Article> Articles { get; set; }
    }
}
