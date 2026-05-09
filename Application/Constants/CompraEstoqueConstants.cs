namespace CRMVEXA.Application.Constants;

/// <summary>
/// Regra alinhada ao front (Estoque.jsx): só compras <c>recebida</c> entram fisicamente no estoque.
/// Estados <c>pendente</c> e <c>aprovada</c> não movimentam saldo; <c>cancelada</c> estorna entradas já registradas.
/// </summary>
public static class CompraEstoqueConstants
{
    public const string StatusRecebida = "recebida";

    public static bool StatusDisparaEntradaFisica(string? status) =>
        string.Equals(status?.Trim(), StatusRecebida, StringComparison.OrdinalIgnoreCase);
}
