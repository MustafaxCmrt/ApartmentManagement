namespace ApartmentManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendInviteEmailAsync(string toEmail, string inviteUrl, string tenantName, CancellationToken ct);
}
