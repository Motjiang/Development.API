using Development.API.Data;
using Development.API.Features.ApplicationUsers.Commands;
using Development.API.Models.Domain;
using Mailjet.Client.Resources;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
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
            };

            var result = await _userManager.CreateAsync(author, request.Password);           
            await _userManager.AddToRoleAsync(author, ApplicationDataSeed.AuthorRole);

            return result.Succeeded ? author : null;
        }
    }
}
