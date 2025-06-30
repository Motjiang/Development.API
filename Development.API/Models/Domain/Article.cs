using System.ComponentModel.DataAnnotations;

namespace Development.API.Models.Domain
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
