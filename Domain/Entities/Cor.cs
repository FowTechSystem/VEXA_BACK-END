namespace CRMVEXA.Domain.Entities;

public class Cor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Cor() { }

    public Cor(string nome)
    {
        Nome = nome;
    }
}
