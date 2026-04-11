namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Lançamento manual de lucro (entrada) ou despesa (saída) no caixa, sem venda/compra.
/// </summary>
public class MovimentacaoCaixaManual
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int? UsuarioId { get; set; }

    /// <summary>Valores esperados no resumo: entrada_manual, saida_manual.</summary>
    public string TipoResumo { get; set; } = string.Empty;

    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Observacoes { get; set; }

    /// <summary>Data do lançamento (dia civil; armazenada em UTC meia-noite).</summary>
    public DateTime DataLancamento { get; set; }

    /// <summary>dinheiro, credito, debito, pix — apenas para entrada.</summary>
    public string? FormaPagamento { get; set; }

    public DateTime CriadoEmUtc { get; set; }

    private MovimentacaoCaixaManual() { }

    public MovimentacaoCaixaManual(
        int empresaId,
        int? usuarioId,
        string tipoResumo,
        decimal valor,
        string descricao,
        string categoria,
        DateTime dataLancamentoUtc,
        string? observacoes,
        string? formaPagamento)
    {
        EmpresaId = empresaId;
        UsuarioId = usuarioId;
        TipoResumo = tipoResumo;
        Valor = valor;
        Descricao = descricao;
        Categoria = categoria;
        DataLancamento = dataLancamentoUtc;
        Observacoes = observacoes;
        FormaPagamento = formaPagamento;
        CriadoEmUtc = DateTime.UtcNow;
    }
}
