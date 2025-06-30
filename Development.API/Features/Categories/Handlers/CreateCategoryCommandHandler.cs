using Development.API.Data;
using Development.API.Features.Categories.Commands;
using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.Categories.Handlers
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.name,
                UrlHandle = request.urlHandle,
                Status = "Active"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.CategoryId;
        }
    }
}
