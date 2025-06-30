using Development.API.Data;
using Development.API.Features.Categories.Commands;
using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.Categories.Handlers
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.categoryId, cancellationToken);

            if (category == null)
                return false;

            category.Name = request.name;
            category.UrlHandle = request.urlHandle;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
