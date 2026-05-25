namespace CRMVEXA.Application.Features.Personalizacao;

public class EmpresaSettingsResponse
{
    public string? LogoUrl { get; set; }
    public string DisplayName { get; set; } = "";
    public string PrimaryColor { get; set; } = "";
    public string SecondaryColor { get; set; } = "";
    public string DefaultTheme { get; set; } = "dark";
    public List<ModuloStatusDto> Modules { get; set; } = new();
}

public class EmpresaSettingsMeResponse
{
    public string? LogoUrl { get; set; }
    public string DisplayName { get; set; } = "";
    public string PrimaryColor { get; set; } = "";
    public string SecondaryColor { get; set; } = "";
    public string DefaultTheme { get; set; } = "dark";
    public List<string> EnabledModules { get; set; } = new();
}

public class ModuloStatusDto
{
    public string Key { get; set; } = "";
    public bool IsEnabled { get; set; }
}

public class AtualizarEmpresaSettingsRequest
{
    public string DisplayName { get; set; } = "";
    public string PrimaryColor { get; set; } = "";
    public string SecondaryColor { get; set; } = "";
    public string DefaultTheme { get; set; } = "dark";
}

public class AtualizarEmpresaModulosRequest
{
    public List<ModuloStatusDto> Modules { get; set; } = new();
}

public class LogoUploadResponse
{
    public string LogoUrl { get; set; } = "";
}
