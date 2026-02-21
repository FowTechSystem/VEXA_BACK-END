using System.Security.Claims;
using CRMVEXA.API.Middleware;

namespace CRMVEXA.Application.Services;

/// <summary>
/// Fornece o EmpresaId efetivo: para dev@gmail.com com X-Empresa-Id, usa o header (validado pelo middleware).
/// Caso contrário, usa empresa_id do token.
/// </summary>
public class EmpresaContext : IEmpresaContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmpresaContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int EmpresaId
    {
        get
        {
            if (!TemEmpresa)
                throw new InvalidOperationException("EmpresaId não disponível no contexto. Verifique se o usuário está autenticado e o token contém o claim 'empresa_id'.");
            return _empresaId!.Value;
        }
    }

    public bool TemEmpresa => _empresaId.HasValue;

    private int? _empresaId
    {
        get
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return null;

            // 1. Valor resolvido pelo middleware (inclui X-Empresa-Id validado para dev)
            if (ctx.Items[EmpresaContextMiddleware.HttpContextKeyEmpresaId] is int mid)
                return mid;

            // 2. Fallback: claim do token
            return ctx.User?.FindFirstValue("empresa_id") is string s && int.TryParse(s, out var id)
                ? id
                : null;
        }
    }
}
