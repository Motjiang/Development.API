using Development.API.Data;
using Development.API.Features.Articles.Commands;
using Development.API.Features.Articles.Queries;
using Development.API.Features.Categories.Commands;
using Development.API.Features.Categories.Queries;
using Development.API.Models.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ApplicationDbContext _context;

        public ArticleController(ISender mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("get-all-public-articles")]
        public async Task<IActionResult> GetAllPublicArticles()
        {
            try
            {
                var allArticles = await _mediator.Send(new GetPublicArticlesQuery());                

                if (!allArticles.Any())
                {
                    return NotFound(new
                    {
                        title = "No Articles Found",
                        message = "There are no matching articles."
                    });
                }

                return Ok(allArticles);
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

        [HttpGet("get-all-articles")]
        public async Task<IActionResult> GetAllArticles([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5, [FromQuery] string searchString = "")
        {
            try
            {
                var allArticles = await _mediator.Send(new GetAllArticlesQuery());

                // Search by title or description
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    allArticles = allArticles
                        .Where(a =>
                            a.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                            a.ShortDescription.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var totalCount = allArticles.Count;

                var pagedArticles = allArticles
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (!pagedArticles.Any())
                {
                    return NotFound(new
                    {
                        title = "No Articles Found",
                        message = "There are no matching articles."
                    });
                }

                return Ok(new
                {
                    data = pagedArticles,
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
        
        [HttpGet("get-article-id/{articleId}")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            try
            {
                var article = await _mediator.Send(new GetArticleByIdQuery(articleId));

                if (article == null)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
                    });
                }
                return Ok(article);
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

        [HttpGet("get-article-url/{urlHandle}")]
        public async Task<IActionResult> GetArticleByUrlHandle(string urlHandle)
        {
            try
            {
                var article = await _mediator.Send(new GetArticleByUrlHandleQuery(urlHandle));

                if (article == null)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
                    });
                }
                return Ok(article);
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
        [HttpPost("write-article")]
        public async Task<IActionResult> WriteArticle([FromBody] WriteArticleCommand command)
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
                if (await CheckArticleContentExistsAsync(command.content))
                {
                    return Conflict(new
                    {
                        title = "Article Already Exists",
                        message = "An Article with the same content already exists."
                    });
                }

                var createdArticleId = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetArticleById), new { articleId = createdArticleId }, null);
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
        [HttpPatch("approve-article/{articleId}")]
        public async Task<IActionResult> ApproveArticle(int articleId)
        {
            try
            {
                var approve = await _mediator.Send(new ApproveArticleCommand(articleId));
                if (!approve)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
                    });
                }
                return Ok();
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
        [HttpPut("update/{articleId}")]
        public async Task<IActionResult> UpdateArticle(int articleId, [FromBody] UpdateArticleCommand command)
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
                if (await CheckArticleContentExistsAsync(command.content, articleId))
                {
                    return Conflict(new
                    {
                        title = "Article Already Exists",
                        message = "An Article with the same content already exists."
                    });
                }

                var updated = await _mediator.Send(command);
                if (!updated)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
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

        [Authorize]
        [HttpPatch("delete/{articleId}")]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            try
            {
                var deleted = await _mediator.Send(new DeleteArticleCommand(articleId));
                if (!deleted)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
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

        [Authorize]
        [HttpPost("category-count")]
        public async Task<IActionResult> GetArticlesTotal()
        {
            try
            {
                var approve = await _mediator.Send(new GetArticleCountQuery());
                if (approve == null)
                {
                    return NotFound(new
                    {
                        title = "Article Not Found",
                        message = "The requested article does not exist."
                    });
                }
                return Ok();
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

        #region
        private async Task<bool> CheckArticleContentExistsAsync(string content, int? excludeArticleId = null)
        {
            if (excludeArticleId.HasValue)
            {
                return await _context.Articles
                    .AnyAsync(a => a.Content == content && a.ArticleId != excludeArticleId.Value);
            }

            return await _context.Articles
                .AnyAsync(a => a.Content == content);
        }
        #endregion
    }
}
