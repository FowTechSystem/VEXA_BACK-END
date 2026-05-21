using CRMVEXA.Domain.Enums;

namespace CRMVEXA.Domain.Entities;

public class BazarProduto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ConsignanteId { get; set; }
    public int SacolaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
    public decimal ValorCompra { get; set; }
    public decimal ValorVenda { get; set; }
    public string? CodigoEtiqueta { get; set; }
    public BazarProdutoStatus Status { get; set; } = BazarProdutoStatus.Disponivel;
    public DateTime DataEntrada { get; set; }
    public DateTime? DataVenda { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public int? VendaId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public DateTime? DeletadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public BazarConsignante Consignante { get; set; } = null!;
    public BazarSacola Sacola { get; set; } = null!;
    public Venda? Venda { get; set; }
    public ICollection<BazarProdutoFoto> Fotos { get; set; } = new List<BazarProdutoFoto>();
    public BazarRepasseItem? RepasseItem { get; set; }
}
