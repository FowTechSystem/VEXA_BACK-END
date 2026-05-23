using CRMVEXA.Domain.Enums;

namespace CRMVEXA.Domain.Entities;

public class EmpresaConfiguracao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string? LogoPath { get; set; }
    public string? LogoFilename { get; set; }
    public string PrimaryColor { get; set; } = "#FACC15";
    public string SecondaryColor { get; set; } = "#1F2937";
    public TemaPadraoEmpresa DefaultTheme { get; set; } = TemaPadraoEmpresa.Dark;
    public string DisplayName { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Usuario? AtualizadoPor { get; set; }
}
