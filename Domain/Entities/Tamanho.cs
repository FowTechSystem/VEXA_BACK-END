namespace CRMVEXA.Domain.Entities;

public class Tamanho
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // P, M, G, GG, 38, 40, etc.

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Tamanho() { }

    public Tamanho(string nome)
    {
        Nome = nome;
    }
}
