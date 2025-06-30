using MediatR;

namespace Development.API.Features.Articles.Commands
{
    public record ApproveArticleCommand(int articleId) : IRequest<bool>;
}
