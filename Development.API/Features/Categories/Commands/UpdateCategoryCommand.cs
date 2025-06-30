using MediatR;

namespace Development.API.Features.Categories.Commands
{
    public record UpdateCategoryCommand(int categoryId, string name, string urlHandle) : IRequest<bool>;
}
