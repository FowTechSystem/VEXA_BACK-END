namespace CRMVEXA.Domain.Entities;

public class Subcategoria
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Empresa Empresa { get; set; } = null!;

    public Categoria Categoria { get; set; } = null!;
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Subcategoria() { }

    public Subcategoria(string nome, int categoriaId)
    {
        Nome = nome;
        CategoriaId = categoriaId;
    }
}
