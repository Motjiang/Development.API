using MediatR;

namespace Development.API.Features.Articles.Commands
{
    public record UpdateArticleCommand(int articleId, string title, string shortDescription, string content, string featuredImageUrl, string urlHandle, List<int> CategoryIds) : IRequest<bool>;
}
