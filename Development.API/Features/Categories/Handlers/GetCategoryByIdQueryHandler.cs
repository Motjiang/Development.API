using Development.API.Data;
using Development.API.Features.Categories.Queries;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Categories.Handlers
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category>
    {
        private readonly ApplicationDbContext _context;

        public GetCategoryByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.Where(c => c.Status != "Deleted").FirstOrDefaultAsync(c => c.CategoryId == request.categoryId, cancellationToken);

            return category;
        }
    }
}
