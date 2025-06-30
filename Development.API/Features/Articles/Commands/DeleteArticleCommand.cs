using MediatR;

namespace Development.API.Features.Articles.Commands
{
    public record DeleteArticleCommand(int articleId) : IRequest<bool>;
}
