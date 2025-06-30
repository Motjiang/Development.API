using Development.API.Features.ApplicationUsers.Commands;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UnlockUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            return result.Succeeded;
        }
    }
}
