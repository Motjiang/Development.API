using Development.API.Features.ApplicationUsers.Commands;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LockUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            var lockoutEndDate = DateTimeOffset.UtcNow.AddDays(5);
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return result.Succeeded;
        }
    }
}
