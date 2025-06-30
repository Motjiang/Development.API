using Development.API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Development.API.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbContextSeed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GenerateContextAsync()
        {
            if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                // applies any pending migration into our database
                await _context.Database.MigrateAsync();
            }

            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = ApplicationDataSeed.AdminRole });
                await _roleManager.CreateAsync(new IdentityRole { Name = ApplicationDataSeed.AuthorRole });
            }

            if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult())
            {
                var administrator = new ApplicationUser
                {
                    FirstName = "Oratile",
                    LastName = "Jones",
                    UserName = ApplicationDataSeed.AdminUserName,
                    Email = ApplicationDataSeed.
                    AdminUserName,
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now,
                    Status = "Active"
                };
                await _userManager.CreateAsync(administrator, "123456");
                await _userManager.AddToRoleAsync(administrator, ApplicationDataSeed.AdminRole);
                await _userManager.AddClaimsAsync(administrator, new Claim[]
                {
                    new Claim(ClaimTypes.Email, administrator.Email),
                    new Claim(ClaimTypes.Surname, administrator.LastName)
                });

                var author = new ApplicationUser
                {
                    FirstName = "Odirile",
                    LastName = "More",
                    UserName = ApplicationDataSeed.AuthorUserName,
                    Email = ApplicationDataSeed.AuthorUserName,
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now,
                    Status = "Active"
                };
                await _userManager.CreateAsync(author, "123456");
                await _userManager.AddToRoleAsync(author, ApplicationDataSeed.AuthorRole);
                await _userManager.AddClaimsAsync(author, new Claim[]
                {
                    new Claim(ClaimTypes.Email, author.Email),
                    new Claim(ClaimTypes.Surname, author.LastName)
                });
            }
        }
    }
}
