using System.Linq;

namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Catálogo digital: link público para cliente ver produtos e enviar pedido via WhatsApp.
/// </summary>
public class Catalogo
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int CriadorId { get; set; }
    public string? CriadorNome { get; set; }
    public string CriadorWhatsApp { get; set; } = string.Empty;
    public string? EmpresaNome { get; set; }
    public string? Slug { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Ativo { get; set; } = true;

    public Empresa Empresa { get; set; } = null!;
    public Usuario Criador { get; set; } = null!;

    private List<ItemCatalogo> _produtos = new();
    public IReadOnlyCollection<ItemCatalogo> Produtos => _produtos.AsReadOnly();

    private Catalogo() { }

    public Catalogo(int empresaId, int criadorId, string criadorWhatsApp)
    {
        EmpresaId = empresaId;
        CriadorId = criadorId;
        CriadorWhatsApp = criadorWhatsApp;
        CriadoEm = DateTime.UtcNow;
    }

    public void AdicionarProduto(ItemCatalogo item)
    {
        _produtos.Add(item);
    }

    public void DefinirSlug(string? slug) => Slug = slug;
    public void DefinirCriadorNome(string? nome) => CriadorNome = nome;
    public void DefinirEmpresaNome(string? nome) => EmpresaNome = nome;
    public void Desativar() => Ativo = false;
}
