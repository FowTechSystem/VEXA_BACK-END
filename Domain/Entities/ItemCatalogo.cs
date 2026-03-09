namespace CRMVEXA.Domain.Entities;

public class ItemCatalogo
{
    public int Id { get; set; }
    public int CatalogoId { get; set; }
    public int ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; } = 1;

    private ItemCatalogo() { }

    public ItemCatalogo(int produtoId, string nome, decimal preco, int quantidade = 1)
    {
        ProdutoId = produtoId;
        Nome = nome;
        Preco = preco;
        Quantidade = quantidade;
    }
}
