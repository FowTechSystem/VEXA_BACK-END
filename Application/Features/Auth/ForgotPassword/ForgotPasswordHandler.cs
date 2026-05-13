using System.Net.Mail;
using CRMVEXA.Application.Features.Auth;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CRMVEXA.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly ICrossTenantResolver _crossTenant;
    private readonly ITenantAppDbContextFactory _dbFactory;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        ICrossTenantResolver crossTenant,
        ITenantAppDbContextFactory dbFactory,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<ForgotPasswordHandler> logger)
    {
        _crossTenant = crossTenant;
        _dbFactory = dbFactory;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(email) || !TryParseEmail(email))
            throw new ArgumentException("Informe um e-mail válido.");

        var snap = await _crossTenant.ResolveUsuarioForPasswordResetAsync(email, cancellationToken).ConfigureAwait(false);
        if (snap == null)
            return new ForgotPasswordResponse();

        await using var ctx = _dbFactory.CreateDbContext(snap.TenantId);

        var usuario = await ctx.Usuarios
            .FirstOrDefaultAsync(u => u.Id == snap.Usuario.Id && u.Ativo, cancellationToken)
            .ConfigureAwait(false);

        if (usuario == null)
            return new ForgotPasswordResponse();

        var hours = int.TryParse(_configuration["PasswordReset:TokenExpirationHours"], out var h) ? h : 24;
        hours = Math.Clamp(hours, 1, 72);

        await RevogarTokensPendentesAsync(ctx, usuario.Id, cancellationToken).ConfigureAwait(false);

        var (plain, hashB64) = PasswordResetTokenCrypto.CreateToken();
        var row = new PasswordResetToken
        {
            UsuarioId = usuario.Id,
            TokenHash = hashB64,
            ExpiresAt = DateTime.UtcNow.AddHours(hours),
            CreatedAt = DateTime.UtcNow
        };
        ctx.PasswordResetTokens.Add(row);
        await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
            await _emailSender.SendAsync(usuario.Email, subject, html, text, cancellationToken).ConfigureAwait(false);
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

    private static async Task RevogarTokensPendentesAsync(AppDbContext ctx, int usuarioId, CancellationToken cancellationToken)
    {
        var antigos = await ctx.PasswordResetTokens
            .Where(t => t.UsuarioId == usuarioId && t.UsedAt == null)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        if (antigos.Count > 0)
            ctx.PasswordResetTokens.RemoveRange(antigos);
    }
}
