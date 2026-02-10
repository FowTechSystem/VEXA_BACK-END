namespace CRMVEXA.Domain.Entities;

public class GeneroProduto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Masculino, Feminino, Unissex

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private GeneroProduto() { }

    public GeneroProduto(string nome)
    {
        Nome = nome;
    }
}
