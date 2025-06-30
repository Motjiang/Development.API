using MediatR;

namespace Development.API.Features.Categories.Commands
{
    public record CreateCategoryCommand(string name, string urlHandle) : IRequest<int>;
}
