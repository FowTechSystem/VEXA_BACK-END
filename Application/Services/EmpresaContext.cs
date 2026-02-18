using System.Security.Claims;

namespace CRMVEXA.Application.Services;

/// <summary>
/// Lê o EmpresaId do claim "empresa_id" do usuário autenticado (JWT).
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

    private int? _empresaId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("empresa_id") is string s
        && int.TryParse(s, out var id)
        ? id
        : null;
}
