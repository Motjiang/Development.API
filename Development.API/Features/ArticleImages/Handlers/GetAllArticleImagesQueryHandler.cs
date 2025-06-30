using Development.API.Data;
using Development.API.Features.ArticleImages.Queries;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.ArticleImages.Handlers
{
    public class GetAllArticleImagesQueryHandler : IRequestHandler<GetAllArticleImagesQuery, List<ArticleImage>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllArticleImagesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ArticleImage>> Handle(GetAllArticleImagesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ArticleImages.OrderByDescending(ai => ai.DateCreated)
                .ToListAsync(cancellationToken);
        }
    }
}
