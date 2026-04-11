using MediatR;

namespace CRMVEXA.Application.Features.Caixa.CriarMovimentacaoManual;

public class CriarMovimentacaoManualRequest : IRequest<CriarMovimentacaoManualResponse>
{
    /// <summary>entrada_lucro ou saida_despesa.</summary>
    public string Tipo { get; set; } = string.Empty;

    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;

    /// <summary>Data no formato yyyy-MM-dd (ISO).</summary>
    public string Data { get; set; } = string.Empty;

    public string? Observacoes { get; set; }
    public string? FormaPagamento { get; set; }
}
