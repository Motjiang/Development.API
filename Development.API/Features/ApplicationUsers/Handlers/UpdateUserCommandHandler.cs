using Development.API.Features.ApplicationUsers.Commands;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;
           
            var result = await _userManager.UpdateAsync(user);
            
            return result.Succeeded ? user : null;
        }
    }
}
