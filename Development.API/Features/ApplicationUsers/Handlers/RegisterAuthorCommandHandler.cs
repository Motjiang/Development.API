using Development.API.Data;
using Development.API.Features.ApplicationUsers.Commands;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Authors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class RegisterAuthorCommandHandler : IRequestHandler<RegisterCommand, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterAuthorCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var author = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                DateCreated = DateTime.Now,
                EmailConfirmed = true,
                Status = "Active",
            };

            var result = await _userManager.CreateAsync(author, request.Password);
            await _userManager.AddToRoleAsync(author, ApplicationDataSeed.AuthorRole);

            return result.Succeeded ? author : null;
        }
    }
}
