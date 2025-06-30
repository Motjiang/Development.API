using MediatR;

namespace Development.API.Features.ApplicationUsers.Commands
{
    public record LockUserCommand(string UserId) : IRequest<bool>;
}
