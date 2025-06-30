using Development.API.Data;
using Development.API.Features.Articles.Queries;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Articles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, List<ArticleDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllArticlesQueryHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ArticleDto>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            var query = _context.Articles
                .Where(a => a.Status != "Deleted");

            if (!isAdmin)
            {
                query = query.Where(a => a.IsVisible && a.AuthorId == user.Id);
            }

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
}
