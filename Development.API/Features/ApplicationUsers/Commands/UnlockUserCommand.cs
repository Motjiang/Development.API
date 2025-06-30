using MediatR;

namespace Development.API.Features.ApplicationUsers.Commands
{
    public record UnlockUserCommand(string UserId) : IRequest<bool>;
}
