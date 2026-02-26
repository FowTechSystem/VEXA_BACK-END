namespace CRMVEXA.Domain.Entities;

public class Marca
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;
    public Empresa Empresa { get; set; } = null!;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    private Marca() { }

    public Marca(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
        DataCadastro = DateTime.UtcNow;
        Ativo = true;
    }
}
