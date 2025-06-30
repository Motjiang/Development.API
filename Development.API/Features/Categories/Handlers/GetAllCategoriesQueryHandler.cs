using Development.API.Data;
using Development.API.Features.Categories.Queries;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Categories.Handlers
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<Category>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllCategoriesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories.Include(c => c.Articles).Where(c => c.Status != "Deleted").ToListAsync(cancellationToken);
        }
    }
}
