using System.ComponentModel.DataAnnotations;

namespace Development.API.Models.Domain
{
    public class ArticleImage
    {
        [Key]   
        public int ArticleImageId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
