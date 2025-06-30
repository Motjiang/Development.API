using Development.API.Data;
using Development.API.Features.ApplicationUsers.Queries;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Authors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<AuthorDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<AuthorDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName != ApplicationDataSeed.AdminUserName && u.Status != "Deleted")
                .ToListAsync(cancellationToken);

            var authorDtos = new List<AuthorDto>();

            foreach (var user in users)
            {
                var isLocked = await _userManager.IsLockedOutAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                authorDtos.Add(new AuthorDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateCreated = user.DateCreated,
                    IsLocked = isLocked,
                    Roles = roles
                });
            }

            return authorDtos;
        }
    }
}
