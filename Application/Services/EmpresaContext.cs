using CRMVEXA.Application.Constants;

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
                throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT (Authorization: Bearer) e: (1) o claim 'empresa_id' no token, ou (2) se for dev (dev@gmail.com), o header X-Empresa-Id com o ID da empresa.");
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

            if (ctx.Items[TenantHttpKeys.EmpresaId] is int mid)
                return mid;

            var empresaClaim = ctx.User?.FindFirst("empresa_id")?.Value;
            return !string.IsNullOrEmpty(empresaClaim) && int.TryParse(empresaClaim, out var id)
                ? id
                : null;
        }
    }
}
