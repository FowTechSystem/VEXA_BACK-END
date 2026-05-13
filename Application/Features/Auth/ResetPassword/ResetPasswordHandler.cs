using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;

namespace CRMVEXA.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, ResetPasswordResult>
{
    private readonly ICrossTenantResolver _crossTenant;

    public ResetPasswordHandler(ICrossTenantResolver crossTenant)
    {
        _crossTenant = crossTenant;
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

        return await _crossTenant.TryResetPasswordAsync(token, senha, cancellationToken).ConfigureAwait(false);
    }
}
