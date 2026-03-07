namespace CRMVEXA.Application.Features.Caixa.GetResumo;

public class GetCaixaResumoResponse
{
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal SaldoAtual { get; set; }
    public decimal TotalEntradasMenosSaldo { get; set; }
    public List<MovimentacaoResumoItem> Movimentacoes { get; set; } = new();
}

public class MovimentacaoResumoItem
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // "venda" ou "compra"
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    /// <summary>Nome do cliente (venda) ou fornecedor (compra).</summary>
    public string? ClienteFornecedor { get; set; }
    /// <summary>Forma de pagamento (vendas).</summary>
    public string? FormaPagamento { get; set; }
    /// <summary>Status da movimentação (Concluída, Concluído, etc.).</summary>
    public string? Status { get; set; }
}
