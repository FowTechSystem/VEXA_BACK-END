using System.Security.Claims;
using CRMVEXA.Application.Constants;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.API.Middleware;

/// <summary>
/// Resolve o EmpresaId efetivo: para dev@gmail.com com header X-Empresa-Id, usa o header (validado).
/// Caso contrário, usa empresa_id do token. Ignora X-Empresa-Id para usuários que não sejam dev.
/// </summary>
public class EmpresaContextMiddleware
{
    public const string HttpContextKeyEmpresaId = "EmpresaContext.EmpresaId";

    private readonly RequestDelegate _next;

    public EmpresaContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            var empresaIdClaim = user.FindFirstValue("empresa_id");

            var headerEmpresaId = context.Request.Headers["X-Empresa-Id"].FirstOrDefault();
            var isDev = !string.IsNullOrEmpty(email) &&
                        string.Equals(email.Trim(), SistemaConstants.EmailAdminSistema, StringComparison.OrdinalIgnoreCase);

            int? resolvedEmpresaId = null;

            if (isDev && !string.IsNullOrWhiteSpace(headerEmpresaId) && int.TryParse(headerEmpresaId.Trim(), out var headerId))
            {
                // Dev enviou X-Empresa-Id: validar que a empresa existe e está ativa
                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                var empresa = await db.Empresas
                    .AsNoTracking()
                    .Where(e => e.Id == headerId && e.Ativo)
                    .Select(e => new { e.Id })
                    .FirstOrDefaultAsync(context.RequestAborted);

                if (empresa != null)
                    resolvedEmpresaId = empresa.Id;
            }

            // Se não resolved (não dev, ou header inválido, ou empresa não existe): usar claim
            if (!resolvedEmpresaId.HasValue && int.TryParse(empresaIdClaim, out var claimId))
                resolvedEmpresaId = claimId;

            if (resolvedEmpresaId.HasValue)
                context.Items[HttpContextKeyEmpresaId] = resolvedEmpresaId.Value;
        }

        await _next(context);
    }
}
