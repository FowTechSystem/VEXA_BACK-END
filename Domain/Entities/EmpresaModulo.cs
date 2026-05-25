namespace CRMVEXA.Domain.Entities;

public class EmpresaModulo
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string ModuleKey { get; set; } = "";
    public bool IsEnabled { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Usuario? AtualizadoPor { get; set; }
}
