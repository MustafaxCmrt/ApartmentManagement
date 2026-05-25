using ApartmentManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApartmentManagement.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendInviteEmailAsync(string toEmail, string inviteUrl, string tenantAdi, CancellationToken ct)
    {
        _logger.LogInformation(
            "Email gönderilecekti: To={To}, Subject=Davet, Body contains URL: {Url}",
            toEmail,
            inviteUrl);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetUrl, CancellationToken ct)
    {
        _logger.LogInformation(
            "Password reset for {To}: {Url}",
            toEmail,
            resetUrl);
        return Task.CompletedTask;
    }
}
