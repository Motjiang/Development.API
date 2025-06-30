using Development.API.Data;
using Development.API.Features.Articles.Queries;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Articles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class GetArticleByUrlQueryHandler : IRequestHandler<GetArticleByUrlHandleQuery, ArticleDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetArticleByUrlQueryHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ArticleDto> Handle(GetArticleByUrlHandleQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            var query = _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Categories)
                .Where(a => a.UrlHandle == request.urlHandle && a.Status != "Deleted");

            if (!isAdmin)
            {
                query = query.Where(a => a.AuthorId == user.Id && a.IsVisible);
            }

            var article = await query
                .Select(a => new ArticleDto
                {
                    ArticleId = a.ArticleId,
                    Title = a.Title,
                    ShortDescription = a.ShortDescription,
                    Content = a.Content,
                    FeaturedImageUrl = a.FeaturedImageUrl,
                    UrlHandle = a.UrlHandle,
                    PublishedDate = a.PublishedDate,
                    AuthorName = a.Author.FirstName + " " + a.Author.LastName,
                    Categories = a.Categories.Select(c => c.Name).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return article;
        }
    }
}
