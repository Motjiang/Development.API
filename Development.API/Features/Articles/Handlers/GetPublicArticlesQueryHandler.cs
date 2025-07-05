using Development.API.Data;
using Development.API.Features.Articles.Queries;
using Development.API.Models.DTOs.Articles;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetPublicArticlesQueryHandler : IRequestHandler<GetPublicArticlesQuery, List<ArticleDto>>
{
    private readonly ApplicationDbContext _context;

    public GetPublicArticlesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArticleDto>> Handle(GetPublicArticlesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Articles
            .Where(a => a.Status != "Deleted" && a.IsVisible);       

        var articles = await query
            .Include(a => a.Author)
            .Include(a => a.Categories)
            .OrderByDescending(a => a.PublishedDate)
            .Select(a => new ArticleDto
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                ShortDescription = a.ShortDescription,
                FeaturedImageUrl = a.FeaturedImageUrl,
                UrlHandle = a.UrlHandle,
                Content = a.Content,
                AuthorName = a.Author.FirstName + " " + a.Author.LastName,
                PublishedDate = a.PublishedDate,
                LastUpdatedDate = a.LastUpdatedDate,
                IsVisible = a.IsVisible,
                Categories = a.Categories.Select(c => c.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return articles;
    }
}
