using Development.API.Models.Domain;
using Development.API.Models.DTOs.Account;
using Development.API.Models.DTOs.Authors;
using MediatR;

namespace Development.API.Features.ApplicationUsers.Queries
{
    public record GetAllUsersQuery() : IRequest<List<AuthorDto>>;
}
