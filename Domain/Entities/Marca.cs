namespace CRMVEXA.Domain.Entities;

public class Marca
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Empresa Empresa { get; set; } = null!;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Marca() { }

    public Marca(string nome)
    {
        Nome = nome;
    }
}
