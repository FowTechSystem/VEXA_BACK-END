namespace CRMVEXA.Domain.Entities;

public class ItemCondicional
{
    public int Id { get; set; }
    public int CondicionalId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemCondicional() { }

    public ItemCondicional(int produtoId, int quantidade, decimal precoUnitario)
    {
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
