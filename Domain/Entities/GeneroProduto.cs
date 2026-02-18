namespace CRMVEXA.Domain.Entities;

public class GeneroProduto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty; // Masculino, Feminino, Unissex
    public Empresa Empresa { get; set; } = null!;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private GeneroProduto() { }

    public GeneroProduto(string nome)
    {
        Nome = nome;
    }
}
