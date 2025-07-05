using Development.API.Data;
using Development.API.Features.Articles.Queries;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class GetArticleCountQueryHandler : IRequestHandler<GetArticleCountQuery, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetArticleCountQueryHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Handle(GetArticleCountQuery request, CancellationToken cancellationToken)
        {
            var currentAuthor = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var count = await _context.Articles
                .Where(a => a.AuthorId == currentAuthor.Id && a.Status != "Deleted")
                .CountAsync(cancellationToken);

            return count;
        }
    }
}
