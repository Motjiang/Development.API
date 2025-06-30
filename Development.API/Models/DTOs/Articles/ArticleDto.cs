namespace Development.API.Models.DTOs.Articles
{
    public class ArticleDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public string Content { get; set; }
        public bool IsVisible { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public List<string> Categories { get; set; }
    }

}
