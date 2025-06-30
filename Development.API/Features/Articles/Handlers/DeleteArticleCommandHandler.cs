using Development.API.Data;
using Development.API.Features.Articles.Commands;
using Development.API.Models.Domain;
using Development.API.Models.DTOs.Account;
using Development.API.Services;
using Mailjet.Client.Resources;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteArticleCommandHandler(ApplicationDbContext context, EmailService emailService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            var article = await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.ArticleId == request.articleId, cancellationToken);            

            if (article == null)
                return false;

            article.Status = "Deleted";
            article.DeletedDate = DateTime.Now;
            article.IsVisible = false;

            await _context.SaveChangesAsync(cancellationToken);

            // 📨 Customize email based on who deleted it
            string subject;
            string body;

            if (!isAdmin)
            {
                subject = "✅ You Deleted Your Article";
                body = $@"
        <p>Hi {article.Author.FirstName},</p>
        <p>You have successfully deleted your article titled <strong>{article.Title}</strong>.</p>
        <p>If this was a mistake, please contact support to restore it.</p>
        <p>– The Editorial Team</p>";
            }
            else
            {
                subject = "🗑️ Your Article Has Been Deleted";
                body = $@"
        <p>Hi {article.Author.FirstName},</p>
        <p>Your article titled <strong>{article.Title}</strong> has been deleted by an administrator.</p>
        <p>If this was unexpected or you would like to appeal this action, please contact the editorial team.</p>
        <p>– The Editorial Team</p>";
            }

            var emailSend = new EmailSendDto(article.Author.Email, subject, body);
            await _emailService.SendEmailAsync(emailSend);

            return true;
        }

    }
}
