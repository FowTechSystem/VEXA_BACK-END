namespace CRMVEXA.Domain.Entities;

public class BazarProdutoFoto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Ordem { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }

    public BazarProduto Produto { get; set; } = null!;
}
