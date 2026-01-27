using CRMVEXA.Application.Features.Vendas.GetListaVendas;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasHandler : IRequestHandler<GetListaVendasRequest, GetListaVendasResponse>
    {
        private readonly AppDbContext _context;

        public GetListaVendasHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaVendasResponse> Handle(GetListaVendasRequest request, CancellationToken cancellationToken)
        {
            var vendas = await _context.Vendas
                .Include(v => v.Itens)
                .ToListAsync(cancellationToken);

            var vendasResponse = vendas.Select(v => new VendaResponse
            {
                Id = v.Id,
                CodigoVenda = v.CodigoVenda,
                ClienteId = v.ClienteId,
                Desconto = v.Desconto,
                FormaPagamento = v.FormaPagamento,
                UsuarioId = v.UsuarioId,
                DataHora = v.DataHora,
                ValorTotal = v.ValorTotal,
                Status = v.Status,
                Itens = v.Itens.Select(i => new ItemVendaResponse
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            }).ToList();

            return new GetListaVendasResponse { Vendas = vendasResponse };
        }
    }
}
