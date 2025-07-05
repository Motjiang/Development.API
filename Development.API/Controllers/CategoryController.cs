using Development.API.Data;
using Development.API.Features.Categories.Commands;
using Development.API.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ApplicationDbContext _context;

        public CategoryController(ISender mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("get-all-navbar-categories")]
        public async Task<IActionResult> GetAllNavbarCategories()
        {
            try
            {
                var allCategories = await _mediator.Send(new GetAllCategoriesQuery());

               
                if (!allCategories.Any())
                {
                    return NotFound(new
                    {
                        title = "No Categories Found",
                        message = "There are no matching categories."
                    });
                }

                return Ok(allCategories);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5, [FromQuery] string searchString = "")
        {
            try
            {
                var allCategories = await _mediator.Send(new GetAllCategoriesQuery());

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    allCategories = allCategories.Where(d => d.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }


                var totalCount = allCategories.Count();

                var pagedCategoriess = allCategories
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                if (!pagedCategoriess.Any())
                {
                    return NotFound(new
                    {
                        title = "No Categories Found",
                        message = "There are no matching categories."
                    });
                }

                return Ok(new
                {
                    data = pagedCategoriess,
                    totalCount = totalCount,
                    pageIndex = pageIndex,
                    pageSize = pageSize
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        [Authorize]
        [HttpGet("get-category/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var category = await _mediator.Send(new GetCategoryByIdQuery(categoryId));

                if (category == null)
                {
                    return NotFound(new
                    {
                        title = "Category Not Found",
                        message = "The requested category does not exist."
                    });
                }
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            if (command == null)
            {
                return BadRequest(new
                {
                    title = "Invalid Request",
                    message = "The request body cannot be null."
                });
            }

            try
            {
                if (await CheckCategoryExistsAsync(command.name, command.urlHandle))
                {
                    return Conflict(new
                    {
                        title = "Category Already Exists",
                        message = "A category with the same name and URL already exists."
                    });
                }

                var createdCategoryId = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategoryId }, null);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("update-category/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] UpdateCategoryCommand command)
        {
            if (command == null)
            {
                return BadRequest(new
                {
                    title = "Invalid Request",
                    message = "The request body cannot be null."
                });
            }

            var newCommand = command with { categoryId = categoryId };

            try
            {
                if (await CheckCategoryExistsAsync(newCommand.name, newCommand.urlHandle, categoryId))
                {
                    return Conflict(new
                    {
                        title = "Category Already Exists",
                        message = "A category with the same name and URL already exists."
                    });
                }

                var updated = await _mediator.Send(newCommand);
                if (!updated)
                {
                    return NotFound(new
                    {
                        title = "Category Not Found",
                        message = "The requested category does not exist."
                    });
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("delete-category/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var deleted = await _mediator.Send(new DeleteCategoryCommand(categoryId));
                if (!deleted)
                {
                    return NotFound(new
                    {
                        title = "Category Not Found",
                        message = "The requested category does not exist."
                    });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    title = "Server Error",
                    message = "An unexpected error occurred. Please contact support."
                });
            }
        }

        #region Helper Methods
        private async Task<bool> CheckCategoryExistsAsync(string name, string url, int? categoryIdToExclude = null)
        {
            return await _context.Categories
                .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.UrlHandle.ToLower() == url.ToLower() && 
                              (!categoryIdToExclude.HasValue || x.CategoryId != categoryIdToExclude.Value));
        }
        #endregion
    }
}
