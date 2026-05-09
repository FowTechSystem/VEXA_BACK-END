using System.Security.Claims;

namespace CRMVEXA.Application.Constants;

/// <summary>
/// Administrador de desenvolvimento (multi-empresa, empresas, header X-Empresa-Id).
/// O JWT é emitido no login com <see cref="ClaimTypes.Email"/> e, quando aplicável,
/// <see cref="ClaimTypes.Role"/> = <see cref="RoleDevAdmin"/> — sempre a partir dos dados do usuário no banco, não do cliente.
/// </summary>
public static class SistemaConstants
{
    public const string EmailAdminSistema = "dev@gmail.com";

    /// <summary>Valor do claim de papel no JWT para o admin de desenvolvimento (opcional; o e-mail também identifica).</summary>
    public const string RoleDevAdmin = "DevAdmin";

    public static bool IsDevAdminEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) &&
        string.Equals(email.Trim(), EmailAdminSistema, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Utilizador autorizado a gerir empresas e enviar <c>X-Empresa-Id</c>: e-mail dev no token ou papel <see cref="RoleDevAdmin"/>.
    /// </summary>
    public static bool IsUsuarioDevAdmin(ClaimsPrincipal user)
    {
        if (IsDevAdminEmail(user.FindFirst(ClaimTypes.Email)?.Value))
            return true;
        foreach (var c in user.Claims)
        {
            if (c.Type != ClaimTypes.Role && c.Type != "role")
                continue;
            if (string.Equals(c.Value, RoleDevAdmin, StringComparison.Ordinal))
                return true;
        }
        return false;
    }
}
