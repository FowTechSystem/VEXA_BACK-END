using MediatR;

namespace CRMVEXA.Application.Features.Auth.ResetPassword;

public class ResetPasswordRequest : IRequest<ResetPasswordResult>
{
    public string Token { get; set; } = string.Empty;
    /// <summary>Preferencial no contrato do front.</summary>
    public string? NewPassword { get; set; }
    /// <summary>Alias aceito pelo front legado.</summary>
    public string? Password { get; set; }

    public string SenhaEfetiva => !string.IsNullOrEmpty(NewPassword) ? NewPassword : Password ?? string.Empty;
}
