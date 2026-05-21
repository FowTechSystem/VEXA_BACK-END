using CRMVEXA.Domain.Enums;

namespace CRMVEXA.Application.Services.Bazar;

public static class BazarStatusHelper
{
    public static string ToApi(BazarProdutoStatus status) => status switch
    {
        BazarProdutoStatus.Disponivel => "disponivel",
        BazarProdutoStatus.Vendida => "vendida",
        BazarProdutoStatus.Devolvida => "devolvida",
        _ => "disponivel"
    };

    public static BazarProdutoStatus? FromApi(string? status) => status?.Trim().ToLowerInvariant() switch
    {
        "disponivel" => BazarProdutoStatus.Disponivel,
        "vendida" => BazarProdutoStatus.Vendida,
        "devolvida" => BazarProdutoStatus.Devolvida,
        _ => null
    };

    public static string RepasseToApi(BazarRepasseStatus status) =>
        status == BazarRepasseStatus.Pago ? "pago" : "pendente";

    public static BazarRepasseStatus? RepasseFromApi(string? status) => status?.Trim().ToLowerInvariant() switch
    {
        "pendente" => BazarRepasseStatus.Pendente,
        "pago" => BazarRepasseStatus.Pago,
        _ => null
    };
}
