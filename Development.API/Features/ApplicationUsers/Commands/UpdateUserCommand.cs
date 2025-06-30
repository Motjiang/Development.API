using Development.API.Models.Domain;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Development.API.Features.ApplicationUsers.Commands
{
    public record UpdateUserCommand : IRequest<ApplicationUser>
    {
        public string Id { get; init; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
        public string FirstName { get; init; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
        public string LastName { get; init; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; init; }
    }
}
