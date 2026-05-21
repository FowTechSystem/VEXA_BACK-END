namespace CRMVEXA.Domain.Entities;

public class BazarSacola
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ConsignanteId { get; set; }
    public DateTime DataCadastro { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public BazarConsignante Consignante { get; set; } = null!;
    public ICollection<BazarProduto> Produtos { get; set; } = new List<BazarProduto>();
}
