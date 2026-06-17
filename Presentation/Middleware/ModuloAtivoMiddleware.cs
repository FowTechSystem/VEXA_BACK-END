using System.Text.Json;
using CRMVEXA.Application.Services.Personalizacao;

namespace CRMVEXA.Presentation.Middleware;

/// <summary>
/// Bloqueia rotas de módulos opcionais desativados para a empresa (403 MODULE_DISABLED).
/// </summary>
public class ModuloAtivoMiddleware
{
    // Cada rota libera se QUALQUER um dos módulos listados estiver ativo.
    private static readonly (string Prefix, string[] ModuleKeys)[] RotasModulos =
    {
        ("/api/bazar", new[] { "bazar" }),
        ("/api/vendas", new[] { "vendas" }),
        ("/api/caixa", new[] { "caixa" }),
        ("/api/caixas", new[] { "caixa" }),
        ("/api/catalogos", new[] { "catalogo" }),
        ("/api/condicional", new[] { "condicional" }),
        ("/api/compras", new[] { "compras" }),
        ("/api/estoque", new[] { "estoque" }),
        // O PDV (vendas) e o Bazar listam itens vendáveis por esta rota; liberar
        // quando qualquer um deles estiver ativo, não só o módulo "produtos".
        // Assim uma loja só-bazar (produtos off) consegue vender no PDV.
        ("/api/produtos", new[] { "produtos", "vendas", "bazar" }),
        ("/api/clientes", new[] { "clientes" }),
        ("/api/fornecedor", new[] { "fornecedores" }),
        ("/api/fornecedores", new[] { "fornecedores" }),
        ("/api/relatorios", new[] { "relatorios" }),
        ("/api/dashboard", new[] { "relatorios" })
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

        var moduleKeys = ResolverModulo(path);
        if (moduleKeys == null)
        {
            await _next(context);
            return;
        }

        var modulos = context.RequestServices.GetRequiredService<EmpresaModuloAccessService>();
        var algumAtivo = false;
        foreach (var key in moduleKeys)
        {
            if (await modulos.EstaAtivoAsync(key, context.RequestAborted))
            {
                algumAtivo = true;
                break;
            }
        }

        if (!algumAtivo)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                sucesso = false,
                error = "MODULE_DISABLED",
                message = $"O módulo '{moduleKeys[0]}' não está habilitado para esta empresa."
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

    private static string[]? ResolverModulo(string path)
    {
        foreach (var (prefix, keys) in RotasModulos)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return keys;
        }
        return null;
    }
}
