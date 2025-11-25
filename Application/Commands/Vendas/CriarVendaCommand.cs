using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Commands.Vendas
{
    public record CriarVendaCommand(
        string CodigoVenda,
        int? ClienteId,
        decimal Desconto,
        string FormaPagamento,
        int UsuarioId,
        List<ItemVendaDto> Itens
    ) : IRequest<int>;
}
