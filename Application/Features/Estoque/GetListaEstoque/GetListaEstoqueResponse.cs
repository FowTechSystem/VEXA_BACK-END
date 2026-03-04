namespace CRMVEXA.Application.Features.Estoque.GetListaEstoque;

public class GetListaEstoqueResponse
{
    public List<ProdutoEstoqueResponse> Produtos { get; set; } = new();
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int TamanhoPagina { get; set; }
}

public class ProdutoEstoqueResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; }
    public int EstoqueMaximo { get; set; }
    public decimal ValorUnitario { get; set; }
}
