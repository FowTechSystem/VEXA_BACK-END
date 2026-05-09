using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Services;

/// <summary>
/// Movimentação física de estoque ligada a compras.
/// Entrada: apenas quando a compra está em status recebida (ver handlers e CompraEstoqueConstants).
/// Idempotente por compra: não duplica linhas de entrada para o mesmo <see cref="Compra.Id"/>.
/// Estorno: ao cancelar compra já recebida, reverte saldo e regista saídas rastreáveis.
/// </summary>
public interface IEntradaEstoquePorCompraService
{
    /// <summary>Entrada por item (Estoque + Movimentação tipo Entrada). Ignora se já existir entrada para esta compra.</summary>
    Task RegistrarEntradaAsync(int empresaId, Compra compra, int usuarioId, string motivo, CancellationToken cancellationToken);

    /// <summary>Reverte entradas desta compra (Saída espelhada). Idempotente.</summary>
    Task EstornarEntradaPorCompraAsync(int empresaId, Compra compra, int usuarioId, CancellationToken cancellationToken);
}
