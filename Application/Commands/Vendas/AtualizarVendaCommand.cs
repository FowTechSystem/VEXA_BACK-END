using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Commands.Vendas
{
    public record AtualizarVendaCommand(
        int Id,
        string CodigoVenda,
        int? ClienteId,
        decimal Desconto,
        string FormaPagamento,
        int UsuarioId,
        List<ItemVendaDto> Itens
    ) : IRequest<bool>;
}
