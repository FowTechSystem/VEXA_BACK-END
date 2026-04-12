using CRMVEXA.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CRMVEXA.Infrastructure.Email;

/// <summary>Quando SMTP não está configurado: loga o envio (em Development inclui o link para testes).</summary>
public class LogOnlyEmailSender : IEmailSender
{
    private readonly ILogger<LogOnlyEmailSender> _logger;
    private readonly IWebHostEnvironment _env;

    public LogOnlyEmailSender(ILogger<LogOnlyEmailSender> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public Task SendAsync(string to, string subject, string htmlBody, string? textBody, CancellationToken cancellationToken = default)
    {
        if (_env.IsDevelopment())
        {
            _logger.LogWarning(
                "[E-mail não enviado — SMTP não configurado] Para: {To} | Assunto: {Subject}\n{text}",
                to, subject, textBody ?? htmlBody);
        }
        else
        {
            _logger.LogWarning("SMTP não configurado: e-mail de redefinição não enviado para {To}. Configure Email:SmtpHost.", to);
        }

        return Task.CompletedTask;
    }
}
