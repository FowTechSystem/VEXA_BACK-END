using CRMVEXA.Application.Features.Auth;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, ResetPasswordResult>
{
    private readonly AppDbContext _context;

    public ResetPasswordHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResetPasswordResult> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var token = request.Token?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(token))
            return ResetPasswordResult.Fail("Token inválido ou expirado.");

        var senha = request.SenhaEfetiva;
        if (string.IsNullOrEmpty(senha) || senha.Length < 6)
            return ResetPasswordResult.Fail("A senha deve ter pelo menos 6 caracteres.");

        if (senha.Length > 256)
            return ResetPasswordResult.Fail("Senha muito longa.");

        var agora = DateTime.UtcNow;
        var hashB64 = PasswordResetTokenCrypto.HashPlainToken(token);
        var row = await _context.PasswordResetTokens
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(
                t => t.TokenHash == hashB64 && t.UsedAt == null && t.ExpiresAt > agora,
                cancellationToken);
        if (row == null)
            return ResetPasswordResult.Fail("Token inválido ou expirado.");

        if (!row.Usuario.Ativo)
            return ResetPasswordResult.Fail("Token inválido ou expirado.");

        row.Usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        row.UsedAt = agora;

        await _context.SaveChangesAsync(cancellationToken);
        return ResetPasswordResult.Ok();
    }
}
