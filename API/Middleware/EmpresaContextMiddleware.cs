using CRMVEXA.Application.Constants;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.API.Middleware;

/// <summary>
/// Resolve o EmpresaId efetivo: para o admin dev com header X-Empresa-Id, usa o header (validado no master).
/// Usuário comum não pode enviar X-Empresa-Id (403). Demais casos usam claim empresa_id do token.
/// </summary>
public class EmpresaContextMiddleware
{
    public const string HttpContextKeyEmpresaId = TenantHttpKeys.EmpresaId;

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
                var master = context.RequestServices.GetRequiredService<MasterDbContext>();
                var tenantOk = await master.Tenants.AsNoTracking()
                    .AnyAsync(e => e.Id == headerId && e.Ativo, context.RequestAborted);
                if (tenantOk)
                    resolvedEmpresaId = headerId;
            }

            if (!resolvedEmpresaId.HasValue && int.TryParse(empresaIdClaim, out var claimId))
                resolvedEmpresaId = claimId;

            if (resolvedEmpresaId.HasValue)
                context.Items[HttpContextKeyEmpresaId] = resolvedEmpresaId.Value;
        }

        await _next(context);
    }
}
