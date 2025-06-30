using Development.API.Data;
using Development.API.Features.Articles.Commands;
using Development.API.Models.DTOs.Account;
using Development.API.Services;
using Mailjet.Client.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Development.API.Features.Articles.Handlers
{
    public class ApproveArticleCommandHandler : IRequestHandler<ApproveArticleCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ApproveArticleCommandHandler(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> Handle(ApproveArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.ArticleId == request.articleId, cancellationToken);

            article.IsVisible = true;
            article.Status = "Approved";
            article.ApprovedDate = DateTime.Now;
            
            await _context.SaveChangesAsync(cancellationToken);

            var subject = "🎉 Your Article Has Been Approved!";
            var body = $@"
            <p>Hi {article.Author.FirstName},</p>
            <p>Your article titled <strong>{article.Title}</strong> has been approved and is now published.</p>
            <p>Thank you for contributing!</p>
            <p>– The Editorial Team</p>";

            var emailSend = new EmailSendDto(article.Author.Email, subject, body);

            await _emailService.SendEmailAsync(emailSend);

            return true;
        }
    }
}
