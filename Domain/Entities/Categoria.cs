namespace CRMVEXA.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Subcategoria> Subcategorias { get; set; } = new List<Subcategoria>();
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Categoria() { }

    public Categoria(string nome)
    {
        Nome = nome;
    }
}
