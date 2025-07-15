using Development.API.Models.DTOs.Articles;
using MediatR;

namespace Development.API.Features.Articles.Queries
{
    public record GetAwaitingPublishQuery() : IRequest<List<ArticleDto>>;
}
