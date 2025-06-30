using Development.API.Features.ArticleImages.Commands;
using Development.API.Features.ArticleImages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Development.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleImageController : ControllerBase
    {   
        private readonly ISender _mediator;

        public ArticleImageController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-all-article-images")]
        public async Task<IActionResult> GetAllArticleImages()
        {
            var articleImages = await _mediator.Send(new GetAllArticleImagesQuery());

            if (articleImages == null || !articleImages.Any())
                return NotFound("No images found.");

            return Ok(articleImages);
        }

        [HttpPost("upload-article-image")]
        public async Task<IActionResult> UploadArticleImage([FromForm] UploadArticleImageCommand command)
        {
            if (command.file == null || command.file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            ValidateFileUpload(command.file);

            try
            {
                var articleImage = await _mediator.Send(command);
                return Ok(articleImage);
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
        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Unsupported file format. Only JPG, JPEG, and PNG are allowed.");
            }

            if (file.Length > 10 * 1024 * 1024) // 10MB
            {
                throw new ArgumentException("File size cannot exceed 10MB.");
            }
        }
        #endregion
    }
}
