using System.Text.Json;
using CRMVEXA.Application.Services.Personalizacao;

namespace CRMVEXA.Presentation.Middleware;

/// <summary>
/// Bloqueia rotas de módulos opcionais desativados para a empresa (403 MODULE_DISABLED).
/// </summary>
public class ModuloAtivoMiddleware
{
    private static readonly (string Prefix, string ModuleKey)[] RotasModulos =
    {
        ("/api/bazar", "bazar"),
        ("/api/vendas", "vendas"),
        ("/api/caixa", "caixa"),
        ("/api/caixas", "caixa"),
        ("/api/catalogos", "catalogo"),
        ("/api/condicional", "condicional"),
        ("/api/compras", "compras"),
        ("/api/estoque", "estoque"),
        ("/api/produtos", "produtos"),
        ("/api/clientes", "clientes"),
        ("/api/fornecedor", "fornecedores"),
        ("/api/fornecedores", "fornecedores"),
        ("/api/relatorios", "relatorios"),
        ("/api/dashboard", "relatorios")
    };

    private readonly RequestDelegate _next;

    public ModuloAtivoMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        if (DeveIgnorar(path))
        {
            await _next(context);
            return;
        }

        var moduleKey = ResolverModulo(path);
        if (moduleKey == null)
        {
            await _next(context);
            return;
        }

        var modulos = context.RequestServices.GetRequiredService<EmpresaModuloAccessService>();
        if (!await modulos.EstaAtivoAsync(moduleKey, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                sucesso = false,
                error = "MODULE_DISABLED",
                message = $"O módulo '{moduleKey}' não está habilitado para esta empresa."
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            return;
        }

        await _next(context);
    }

    private static bool DeveIgnorar(string path)
    {
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return true;
        if (path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
            return true;
        if (path.StartsWith("/api/dev/", StringComparison.OrdinalIgnoreCase))
            return true;
        if (path.StartsWith("/api/companies/me/settings", StringComparison.OrdinalIgnoreCase))
            return true;
        if (path.StartsWith("/api/empresas", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    private static string? ResolverModulo(string path)
    {
        foreach (var (prefix, key) in RotasModulos)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return key;
        }
        return null;
    }
}
