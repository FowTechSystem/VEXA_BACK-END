using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Services;

/// <summary>
/// Registra entrada na tabela Estoques e em MovimentacoesEstoque para os itens de uma compra já persistida (Compra.Id definido).
/// </summary>
public interface IEntradaEstoquePorCompraService
{
    Task RegistrarEntradaAsync(int empresaId, Compra compra, int usuarioId, string motivo, CancellationToken cancellationToken);
}
