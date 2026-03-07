namespace CRMVEXA.Application.Features.Vendas.CreateVenda;

/// <summary>
/// Resposta do POST /api/vendas (spec Caixa).
/// </summary>
public class CreateVendaResponse
{
    public int Id { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal Desconto { get; set; }
    public string Status { get; set; } = "finalizada";
    public List<CreateVendaItemResponse> Itens { get; set; } = new();
}

public class CreateVendaItemResponse
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public CreateVendaProdutoRefResponse? Produto { get; set; }
}

public class CreateVendaProdutoRefResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
