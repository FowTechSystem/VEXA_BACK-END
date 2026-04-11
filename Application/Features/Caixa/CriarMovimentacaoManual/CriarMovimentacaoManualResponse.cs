namespace CRMVEXA.Application.Features.Caixa.CriarMovimentacaoManual;

public class CriarMovimentacaoManualResponse
{
    public int Id { get; set; }
    /// <summary>entrada_manual ou saida_manual (mesmo valor do GET resumo).</summary>
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string? Observacoes { get; set; }
    public string? FormaPagamento { get; set; }
    /// <summary>entrada ou saida — alinhado ao GET /caixa/resumo.</summary>
    public string Natureza { get; set; } = string.Empty;
}
