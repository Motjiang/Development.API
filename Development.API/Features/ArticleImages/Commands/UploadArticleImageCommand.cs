using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.ArticleImages.Commands
{
    public record UploadArticleImageCommand(IFormFile file, string fileName, string fileExtension, string title, string url) : IRequest<ArticleImage>;
}
