using System.Net;
using System.Net.Mail;
using CRMVEXA.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CRMVEXA.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, string? textBody, CancellationToken cancellationToken = default)
    {
        var host = _configuration["Email:SmtpHost"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning("Email:SmtpHost vazio; não foi possível enviar para {To}", to);
            return;
        }

        var port = int.TryParse(_configuration["Email:SmtpPort"], out var p) ? p : 587;
        var user = _configuration["Email:SmtpUser"];
        var password = _configuration["Email:SmtpPassword"];
        var enableSsl = !bool.TryParse(_configuration["Email:EnableSsl"], out var ssl) || ssl;
        var from = _configuration["Email:FromAddress"] ?? user ?? "noreply@localhost";
        var fromName = _configuration["Email:FromName"] ?? "VEXA CRM";

        using var message = new MailMessage
        {
            From = new MailAddress(from, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl
        };
        if (!string.IsNullOrEmpty(user))
            client.Credentials = new NetworkCredential(user, password);

        await client.SendMailAsync(message, cancellationToken);
    }
}
