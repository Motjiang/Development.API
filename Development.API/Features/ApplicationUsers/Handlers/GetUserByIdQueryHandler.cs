using Development.API.Data;
using Development.API.Features.ApplicationUsers.Queries;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Authors;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.ApplicationUsers.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ModifyAuthorDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ModifyAuthorDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var authorDto = await _userManager.Users
                .Where(u => u.UserName != ApplicationDataSeed.AdminUserName && u.Id == request.applicationUserId && u.Status != "Deleted")
                .Select(u => new ModifyAuthorDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Roles = string.Join(", ", _userManager.GetRolesAsync(u).Result)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return authorDto;
        }
    }
}
