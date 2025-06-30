using MediatR;

namespace Development.API.Features.Categories.Commands
{
    public record DeleteCategoryCommand(int categoryId) : IRequest<bool>;
}
