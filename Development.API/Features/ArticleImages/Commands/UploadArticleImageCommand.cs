using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.ArticleImages.Commands
{
    public record UploadArticleImageCommand(IFormFile file, string fileName, string title) : IRequest<ArticleImage>;
}
