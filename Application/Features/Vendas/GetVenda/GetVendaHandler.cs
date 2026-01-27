using System;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaHandler : IRequestHandler<GetVendaRequest, GetVendaResponse>
    {
        private readonly AppDbContext _context;

        public GetVendaHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetVendaResponse> Handle(GetVendaRequest request, CancellationToken cancellationToken)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            return new GetVendaResponse
            {
                Id = venda.Id,
                CodigoVenda = venda.CodigoVenda,
                ClienteId = venda.ClienteId,
                Desconto = venda.Desconto,
                FormaPagamento = venda.FormaPagamento,
                UsuarioId = venda.UsuarioId,
                DataHora = venda.DataHora,
                ValorTotal = venda.ValorTotal,
                Status = venda.Status,
                Itens = venda.Itens.Select(i => new ItemVendaResponse
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
    }
}
