using Development.API.Models.DTOs.Articles;
using MediatR;

namespace Development.API.Features.Articles.Queries
{
    public record GetPublicArticlesQuery() : IRequest<List<ArticleDto>>;
}
