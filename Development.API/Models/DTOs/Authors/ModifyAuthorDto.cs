using System.ComponentModel.DataAnnotations;

namespace Development.API.Models.DTOs.Authors
{
    public class ModifyAuthorDto
    {
        public string Id { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid username address")]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Password { get; set; }
        [Required]
        public string Roles { get; set; }
    }
}
