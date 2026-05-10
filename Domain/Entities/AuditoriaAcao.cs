namespace CRMVEXA.Domain.Entities;

public class AuditoriaAcao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int UsuarioId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? RecursoId { get; set; }
    public string? AntesJson { get; set; }
    public string? DepoisJson { get; set; }
    public string? EnderecoIp { get; set; }
    public DateTime CriadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
