using Development.API.Models.DTOs.Articles;
using MediatR;

namespace Development.API.Features.Articles.Queries
{
    public record GetArticleByUrlHandleQuery(string urlHandle) : IRequest<ArticleDto>;
}
