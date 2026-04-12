namespace CRMVEXA.Application.Services;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, string? textBody, CancellationToken cancellationToken = default);
}
