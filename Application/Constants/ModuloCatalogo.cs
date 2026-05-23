using System.Text.RegularExpressions;

namespace CRMVEXA.Application.Constants;

public sealed record ModuloDefinicao(
    string Key,
    string Label,
    bool Fixed,
    IReadOnlyList<string>? Requires = null,
    IReadOnlyList<string>? RequiresAny = null);

public static class ModuloCatalogo
{
    private static readonly Regex HexColor = new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public static IReadOnlyList<ModuloDefinicao> Todos { get; } = new[]
    {
        new ModuloDefinicao("inicio", "Início", Fixed: true),
        new ModuloDefinicao("configuracoes", "Configurações", Fixed: true),
        new ModuloDefinicao("usuarios", "Usuários", Fixed: true),
        new ModuloDefinicao("empresas", "Empresas", Fixed: true),
        new ModuloDefinicao("bazar", "Bazar", Fixed: false),
        new ModuloDefinicao("repasses", "Repasses", Fixed: false, Requires: new[] { "bazar" }),
        new ModuloDefinicao("caixa", "Caixa", Fixed: false, RequiresAny: new[] { "vendas", "bazar" }),
        new ModuloDefinicao("catalogo", "Catálogo Digital", Fixed: false, Requires: new[] { "produtos" }),
        new ModuloDefinicao("condicional", "Condicional", Fixed: false, RequiresAny: new[] { "produtos", "bazar" }),
        new ModuloDefinicao("vendas", "Vendas", Fixed: false, RequiresAny: new[] { "produtos", "bazar" }),
        new ModuloDefinicao("compras", "Compras", Fixed: false, Requires: new[] { "produtos" }),
        new ModuloDefinicao("estoque", "Estoque", Fixed: false, Requires: new[] { "produtos" }),
        new ModuloDefinicao("fiscal", "Fiscal", Fixed: false, Requires: new[] { "vendas" }),
        new ModuloDefinicao("produtos", "Produtos", Fixed: false),
        new ModuloDefinicao("clientes", "Clientes", Fixed: false),
        new ModuloDefinicao("fornecedores", "Fornecedores", Fixed: false, Requires: new[] { "compras" }),
        new ModuloDefinicao("relatorios", "Relatórios", Fixed: false)
    };

    private static readonly Dictionary<string, ModuloDefinicao> PorChave =
        Todos.ToDictionary(m => m.Key, StringComparer.OrdinalIgnoreCase);

    public static bool TryObter(string key, out ModuloDefinicao definicao) =>
        PorChave.TryGetValue(key, out definicao!);

    public static bool EhHexValido(string? color) =>
        !string.IsNullOrWhiteSpace(color) && HexColor.IsMatch(color);

    public static string NormalizarTema(string? tema) =>
        string.Equals(tema, "light", StringComparison.OrdinalIgnoreCase) ? "light" : "dark";

    public static void ValidarModulos(IReadOnlyDictionary<string, bool> estadoEntrada)
    {
        var estado = estadoEntrada.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

        foreach (var kv in estadoEntrada)
        {
            if (PorChave.TryGetValue(kv.Key, out var def) && def.Fixed && !kv.Value)
            {
                throw new ModuloValidacaoException(
                    "FIXED_MODULE",
                    $"O módulo '{def.Label}' é fixo e não pode ser desativado.");
            }
        }

        foreach (var def in Todos.Where(d => d.Fixed))
            estado[def.Key] = true;

        var chavesFaltando = Todos.Select(m => m.Key).Where(k => !estado.ContainsKey(k)).ToList();
        if (chavesFaltando.Count > 0)
        {
            throw new ModuloValidacaoException(
                "INCOMPLETE_MODULES",
                $"Informe todos os módulos do catálogo. Faltando: {string.Join(", ", chavesFaltando)}.");
        }

        var chavesDesconhecidas = estadoEntrada.Keys
            .Where(k => !PorChave.ContainsKey(k))
            .ToList();
        if (chavesDesconhecidas.Count > 0)
            throw new ModuloValidacaoException(
                "UNKNOWN_MODULE",
                $"Módulo(s) desconhecido(s): {string.Join(", ", chavesDesconhecidas)}.");

        foreach (var (key, enabled) in estado.Where(x => !x.Value))
        {
            if (!PorChave.TryGetValue(key, out var desativando))
                continue;

            var dependentes = Todos
                .Where(m => estado.TryGetValue(m.Key, out var mOn) && mOn)
                .Where(m => DependeDe(m, key, estado))
                .Select(m => m.Key)
                .ToList();

            if (dependentes.Count == 0)
                continue;

            var labels = dependentes.Select(k => PorChave[k].Label);
            throw new ModuloValidacaoException(
                "DEPENDENCY_VIOLATION",
                $"Não é possível desativar '{desativando.Label}' porque os módulos ativos '{string.Join(", ", labels)}' dependem dele.",
                dependentes);
        }

        foreach (var (key, enabled) in estado)
        {
            if (!enabled || !PorChave.TryGetValue(key, out var def))
                continue;

            if (def.Requires != null)
            {
                foreach (var req in def.Requires)
                {
                    if (!estado.TryGetValue(req, out var reqOn) || !reqOn)
                    {
                        throw new ModuloValidacaoException(
                            "DEPENDENCY_VIOLATION",
                            $"Não é possível ativar '{def.Label}' sem ativar '{PorChave[req].Label}'.");
                    }
                }
            }

            if (def.RequiresAny != null)
            {
                var algum = def.RequiresAny.Any(r => estado.TryGetValue(r, out var on) && on);
                if (!algum)
                {
                    var nomes = string.Join(", ", def.RequiresAny.Select(r => PorChave[r].Label));
                    throw new ModuloValidacaoException(
                        "DEPENDENCY_VIOLATION",
                        $"Não é possível ativar '{def.Label}' sem ativar pelo menos um de: {nomes}.");
                }
            }
        }
    }

    public static Dictionary<string, bool> MesclarComPadrao(IEnumerable<EmpresaModuloEstado>? modulosDb)
    {
        var map = Todos.ToDictionary(m => m.Key, m => m.Fixed, StringComparer.OrdinalIgnoreCase);
        if (modulosDb == null)
            return map;

        foreach (var m in modulosDb)
        {
            if (PorChave.ContainsKey(m.Key))
                map[m.Key] = m.IsEnabled || PorChave[m.Key].Fixed;
        }

        foreach (var def in Todos.Where(d => d.Fixed))
            map[def.Key] = true;

        return map;
    }

    private static bool DependeDe(ModuloDefinicao modulo, string chave, IReadOnlyDictionary<string, bool> estado)
    {
        if (modulo.Requires?.Contains(chave, StringComparer.OrdinalIgnoreCase) == true)
            return true;
        if (modulo.RequiresAny == null || !modulo.RequiresAny.Contains(chave, StringComparer.OrdinalIgnoreCase))
            return false;

        var outroAtivo = modulo.RequiresAny
            .Where(r => !string.Equals(r, chave, StringComparison.OrdinalIgnoreCase))
            .Any(r => estado.TryGetValue(r, out var on) && on);
        return !outroAtivo;
    }
}

public sealed record EmpresaModuloEstado(string Key, bool IsEnabled);

public class ModuloValidacaoException : Exception
{
    public string ErrorCode { get; }
    public IReadOnlyList<string>? BlockingModules { get; }

    public ModuloValidacaoException(string errorCode, string message, IReadOnlyList<string>? blockingModules = null)
        : base(message)
    {
        ErrorCode = errorCode;
        BlockingModules = blockingModules;
    }
}
