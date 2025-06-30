using Development.API.Data;
using Development.API.Features.Articles.Commands;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateArticleCommandHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            // Get current user (author)
            var currentAuthor = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Fetch existing article
            var article = await _context.Articles
                .Include(a => a.Categories)
                .FirstOrDefaultAsync(a => a.ArticleId == request.articleId, cancellationToken);

            if (article == null || article.AuthorId != currentAuthor.Id)
            {
                return false; // Optionally throw UnauthorizedAccessException
            }

            // ✅ Update only allowed fields
            article.Title = request.title;
            article.Content = request.content;
            article.ShortDescription = request.shortDescription;
            article.FeaturedImageUrl = request.featuredImageUrl;
            article.UrlHandle = request.urlHandle;
            article.LastUpdatedDate = DateTime.Now;

            // ✅ Update categories if provided
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categories = await _context.Categories
                    .Where(c => request.CategoryIds.Contains(c.CategoryId))
                    .ToListAsync(cancellationToken);

                article.Categories = categories;
            }

            // No need to call _context.Update(article) if the entity is tracked
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
