using Development.API.Data;
using Development.API.Features.ArticleImages.Commands;
using Development.API.Models.Domain;
using MediatR;

namespace Development.API.Features.ArticleImages.Handlers
{
    public class UploadArticleImageCommandHandler : IRequestHandler<UploadArticleImageCommand, ArticleImage>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploadArticleImageCommandHandler(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ArticleImage> Handle(UploadArticleImageCommand request, CancellationToken cancellationToken)
        {
            var file = request.file;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            var articleImage = new ArticleImage
            {
                FileName = request.fileName,
                FileExtension = fileExtension,
                Title = request.title,
                DateCreated = DateTime.Now
            };

            // Save to disk
            var localPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{articleImage.FileName}{fileExtension}");
            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            // Generate public URL
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            articleImage.Url = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{articleImage.FileName}{fileExtension}";

            // Save to DB
            await _context.ArticleImages.AddAsync(articleImage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return articleImage;
        }
    }
}
