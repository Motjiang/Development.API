using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.Categories.Queries
{
    public record GetAllCategoriesQuery() : IRequest<IEnumerable<Category>>;
}
