using CRMVEXA.Application.Features.Vendas.GetListaVendas;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasHandler : IRequestHandler<GetListaVendasRequest, GetListaVendasResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetListaVendasHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetListaVendasResponse> Handle(GetListaVendasRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var query = _context.Vendas.Where(v => v.EmpresaId == empresaId).Include(v => v.Itens).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                var valor = request.Valor.ToLower();
                query = query.Where(v => v.CodigoVenda.ToLower().Contains(valor));
            }

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 50);

            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var vendas = await query
                .OrderByDescending(v => v.DataHora)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
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

            return new GetListaVendasResponse
            {
                Vendas = vendasResponse,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = tamanho
            };
        }
    }
}
