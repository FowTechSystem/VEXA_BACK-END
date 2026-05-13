namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Empresa (tenant). Em database-per-tenant o Id costuma coincidir com o registro em Tenants no banco master.
/// </summary>
public class Empresa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? CNPJ { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public ICollection<Caixa> Caixas { get; set; } = new List<Caixa>();
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();

    private Empresa() { }

    public Empresa(string nome, DateTime dataCadastro)
    {
        Nome = nome;
        DataCadastro = dataCadastro;
    }
}
