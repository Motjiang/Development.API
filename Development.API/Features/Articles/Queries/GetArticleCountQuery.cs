using MediatR;

namespace Development.API.Features.Articles.Queries
{
    public record GetArticleCountQuery() : IRequest<int>;
}
