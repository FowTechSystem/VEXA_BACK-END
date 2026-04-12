using System.Net.Mail;
using CRMVEXA.Application.Features.Auth;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CRMVEXA.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        AppDbContext context,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<ForgotPasswordHandler> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(email) || !TryParseEmail(email))
            throw new ArgumentException("Informe um e-mail válido.");

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Ativo, cancellationToken);

        if (usuario == null)
            return new ForgotPasswordResponse();

        var hours = int.TryParse(_configuration["PasswordReset:TokenExpirationHours"], out var h) ? h : 24;
        hours = Math.Clamp(hours, 1, 72);

        await RevogarTokensPendentesAsync(usuario.Id, cancellationToken);

        var (plain, hashB64) = PasswordResetTokenCrypto.CreateToken();
        var row = new PasswordResetToken
        {
            UsuarioId = usuario.Id,
            TokenHash = hashB64,
            ExpiresAt = DateTime.UtcNow.AddHours(hours),
            CreatedAt = DateTime.UtcNow
        };
        _context.PasswordResetTokens.Add(row);
        await _context.SaveChangesAsync(cancellationToken);

        var baseUrl = (_configuration["FrontendBaseUrl"] ?? "http://localhost:5173").TrimEnd('/');
        var link = $"{baseUrl}/redefinir-senha?token={Uri.EscapeDataString(plain)}";

        var subject = "Redefinição de senha — VEXA";
        var html = $"""
            <p>Olá,</p>
            <p>Recebemos um pedido para redefinir a senha da sua conta no VEXA CRM.</p>
            <p><a href="{System.Net.WebUtility.HtmlEncode(link)}">Clique aqui para definir uma nova senha</a></p>
            <p>Se você não solicitou, ignore este e-mail.</p>
            <p>Este link expira em {hours} hora(s).</p>
            """;
        var text = $"Redefinição de senha VEXA. Acesse: {link}";

        try
        {
            await _emailSender.SendAsync(usuario.Email, subject, html, text, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de redefinição para {Email}", usuario.Email);
        }

        return new ForgotPasswordResponse();
    }

    private static bool TryParseEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task RevogarTokensPendentesAsync(int usuarioId, CancellationToken cancellationToken)
    {
        var antigos = await _context.PasswordResetTokens
            .Where(t => t.UsuarioId == usuarioId && t.UsedAt == null)
            .ToListAsync(cancellationToken);
        if (antigos.Count > 0)
            _context.PasswordResetTokens.RemoveRange(antigos);
    }
}
