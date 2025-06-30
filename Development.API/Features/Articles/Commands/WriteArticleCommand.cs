using MediatR;

namespace Development.API.Features.Articles.Commands
{
    public record WriteArticleCommand(string title, string shortDescription, string content, string featuredImageUrl, string urlHandle, List<int> categoryIds) : IRequest<int>;
}
