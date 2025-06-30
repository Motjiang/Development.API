using Development.API.Data;
using Development.API.Features.Articles.Commands;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Account;
using Development.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Development.API.Features.Articles.Handlers
{
    public class WriteArticleCommandHandler : IRequestHandler<WriteArticleCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;

        public WriteArticleCommandHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<int> Handle(WriteArticleCommand request, CancellationToken cancellationToken)
        {
            var currentAuthor = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var article = new Article
            {
                Title = request.title,
                Content = request.content,
                ShortDescription = request.shortDescription,
                FeaturedImageUrl = request.featuredImageUrl,
                UrlHandle = request.urlHandle,
                PublishedDate = DateTime.Now,
                IsVisible = false,
                AuthorId = currentAuthor.Id,
                Categories = new List<Category>(),
                Status = "Published"
            };

            // Add categories to the article
            if (request.categoryIds != null && request.categoryIds.Any())
            {
                foreach (var categoryId in request.categoryIds)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        article.Categories.Add(category);
                    }
                }
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync(cancellationToken);

            var subject = "📥 Article Submission Received";
            var body = $@"
            <p>Hi {currentAuthor.FirstName},</p>
            <p>Your article titled <strong>{article.Title}</strong> has been received and is pending editorial approval.</p>
            <p>Once approved, it will be published online.</p>
            <p>– The Editorial Team</p>";

            var emailSend = new EmailSendDto(currentAuthor.Email, subject, body);
            await _emailService.SendEmailAsync(emailSend);

            return article.ArticleId;
        }
    }
}
