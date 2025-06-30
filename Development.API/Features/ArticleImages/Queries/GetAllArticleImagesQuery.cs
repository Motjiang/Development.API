using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.ArticleImages.Queries
{
    public record GetAllArticleImagesQuery() : IRequest<List<ArticleImage>>;   
}
