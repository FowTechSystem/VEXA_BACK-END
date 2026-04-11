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
    public string Tipo { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    /// <summary>Nome do cliente (venda) ou fornecedor (compra).</summary>
    public string? ClienteFornecedor { get; set; }
    /// <summary>Forma de pagamento (vendas e entrada manual).</summary>
    public string? FormaPagamento { get; set; }
    /// <summary>Status da movimentação (Concluída, Concluído, etc.).</summary>
    public string? Status { get; set; }
    /// <summary>Id da venda quando <see cref="Tipo"/> é venda.</summary>
    public int? VendaId { get; set; }
    /// <summary>Id da compra quando <see cref="Tipo"/> é compra.</summary>
    public int? CompraId { get; set; }
    /// <summary>Categoria informada em lançamentos manuais.</summary>
    public string? Categoria { get; set; }
    /// <summary>Fluxo do caixa: entrada ou saida (§2.1 handoff — alias útil para o front).</summary>
    public string Natureza { get; set; } = string.Empty;
}
