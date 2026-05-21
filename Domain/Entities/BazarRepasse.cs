using CRMVEXA.Domain.Enums;

namespace CRMVEXA.Domain.Entities;

public class BazarRepasse
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ConsignanteId { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataGeracao { get; set; }
    public DateTime? DataPagamento { get; set; }
    public BazarRepasseStatus Status { get; set; } = BazarRepasseStatus.Pendente;
    public string? FormaPagamento { get; set; }
    public string? Observacao { get; set; }
    public int CriadoPor { get; set; }
    public int? PagoPor { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public DateTime? DeletadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public BazarConsignante Consignante { get; set; } = null!;
    public Usuario CriadoPorUsuario { get; set; } = null!;
    public Usuario? PagoPorUsuario { get; set; }
    public ICollection<BazarRepasseItem> Itens { get; set; } = new List<BazarRepasseItem>();
}
