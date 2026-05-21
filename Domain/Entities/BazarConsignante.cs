namespace CRMVEXA.Domain.Entities;

public class BazarConsignante
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public DateTime? DeletadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public ICollection<BazarSacola> Sacolas { get; set; } = new List<BazarSacola>();
    public ICollection<BazarProduto> Produtos { get; set; } = new List<BazarProduto>();
    public ICollection<BazarRepasse> Repasses { get; set; } = new List<BazarRepasse>();
}
