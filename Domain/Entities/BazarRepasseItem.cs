namespace CRMVEXA.Domain.Entities;

public class BazarRepasseItem
{
    public int Id { get; set; }
    public int RepasseId { get; set; }
    public int ProdutoId { get; set; }
    public decimal Valor { get; set; }

    public BazarRepasse Repasse { get; set; } = null!;
    public BazarProduto Produto { get; set; } = null!;
}
