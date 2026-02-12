using CRMVEXA.Application.Features.Compras.GetListaCompras;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasHandler : IRequestHandler<GetListaComprasRequest, GetListaComprasResponse>
    {
        private readonly AppDbContext _context;

        public GetListaComprasHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaComprasResponse> Handle(GetListaComprasRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Compras.Include(c => c.Itens).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                var valor = request.Valor.ToLower();
                query = query.Where(c => c.CodigoCompra.ToLower().Contains(valor) ||
                                        (c.Observacoes != null && c.Observacoes.ToLower().Contains(valor)));
            }

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 50);

            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var compras = await query
                .OrderByDescending(c => c.DataHora)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);

            var comprasResponse = new List<CompraResponse>();

            foreach (var compra in compras)
            {
                var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compra.FornecedorId }, cancellationToken);
                var usuario = await _context.Usuarios.FindAsync(new object[] { compra.UsuarioId }, cancellationToken);

                var compraResponse = new CompraResponse
                {
                    Id = compra.Id,
                    CodigoCompra = compra.CodigoCompra,
                    FornecedorId = compra.FornecedorId,
                    FornecedorNome = fornecedor?.Nome,
                    UsuarioId = compra.UsuarioId,
                    UsuarioNome = usuario?.Nome,
                    DataHora = compra.DataHora,
                    ValorTotal = compra.ValorTotal,
                    Observacoes = compra.Observacoes,
                    Status = compra.Status,
                    Itens = new List<ItemCompraResponse>()
                };

                foreach (var item in compra.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(new object[] { item.ProdutoId }, cancellationToken);
                    compraResponse.Itens.Add(new ItemCompraResponse
                    {
                        Id = item.Id,
                        CompraId = item.CompraId,
                        ProdutoId = item.ProdutoId,
                        ProdutoNome = produto?.Nome,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario,
                        Subtotal = item.Subtotal
                    });
                }

                comprasResponse.Add(compraResponse);
            }

            return new GetListaComprasResponse
            {
                Compras = comprasResponse,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = tamanho
            };
        }
    }
}
