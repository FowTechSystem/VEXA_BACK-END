using System.Text.Json.Serialization;

namespace CRMVEXA.Application.Features.Personalizacao;

// O frontend (companySettingsService.js) fala snake_case nestes endpoints
// (display_name, primary_color, enabled_modules...). O resto da API usa camelCase,
// então marcamos explicitamente o JSON aqui para o binding/serialização baterem.

public class EmpresaSettingsResponse
{
    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
    [JsonPropertyName("primary_color")] public string PrimaryColor { get; set; } = "";
    [JsonPropertyName("secondary_color")] public string SecondaryColor { get; set; } = "";
    [JsonPropertyName("default_theme")] public string DefaultTheme { get; set; } = "dark";
    [JsonPropertyName("enabled_modules")] public List<string> EnabledModules { get; set; } = new();
}

public class EmpresaSettingsMeResponse
{
    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
    [JsonPropertyName("primary_color")] public string PrimaryColor { get; set; } = "";
    [JsonPropertyName("secondary_color")] public string SecondaryColor { get; set; } = "";
    [JsonPropertyName("default_theme")] public string DefaultTheme { get; set; } = "dark";
    [JsonPropertyName("enabled_modules")] public List<string> EnabledModules { get; set; } = new();
}

public class AtualizarEmpresaSettingsRequest
{
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
    [JsonPropertyName("primary_color")] public string PrimaryColor { get; set; } = "";
    [JsonPropertyName("secondary_color")] public string SecondaryColor { get; set; } = "";
    [JsonPropertyName("default_theme")] public string DefaultTheme { get; set; } = "dark";
}

public class AtualizarEmpresaModulosRequest
{
    /// <summary>Chaves dos módulos que devem ficar ativos (fixos são sempre incluídos).</summary>
    [JsonPropertyName("enabled_modules")] public List<string> EnabledModules { get; set; } = new();
}

public class LogoUploadResponse
{
    [JsonPropertyName("logo_url")] public string LogoUrl { get; set; } = "";
}
