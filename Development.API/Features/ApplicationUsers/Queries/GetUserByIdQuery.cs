using Development.API.Models.DTOs.Authors;
using MediatR;

namespace Development.API.Features.ApplicationUsers.Queries
{
    public record GetUserByIdQuery(string applicationUserId) : IRequest<ModifyAuthorDto>;
}
