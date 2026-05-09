using System.Security.Claims;
using CRMVEXA.Application.Constants;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.API.Middleware;

/// <summary>
/// Resolve o EmpresaId efetivo: para o admin dev com header X-Empresa-Id, usa o header (validado no banco).
/// Usuário comum não pode enviar X-Empresa-Id (403). Demais casos usam claim empresa_id do token.
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
            var empresaIdClaim = user.FindFirst("empresa_id")?.Value;

            var headerEmpresaId = context.Request.Headers["X-Empresa-Id"].FirstOrDefault();
            var isDev = SistemaConstants.IsUsuarioDevAdmin(user);

            if (!string.IsNullOrWhiteSpace(headerEmpresaId) && !isDev)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Acesso restrito ao administrador de desenvolvimento."
                }, context.RequestAborted);
                return;
            }

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
