using Development.API.Data;
using Development.API.Features.Categories.Commands;
using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.Categories.Handlers
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.categoryId, cancellationToken);

            if (category == null)
                return false;

            category.Status = "Deleted";

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
