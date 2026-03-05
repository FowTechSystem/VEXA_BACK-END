using System;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaHandler : IRequestHandler<GetVendaRequest, GetVendaResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetVendaHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetVendaResponse> Handle(GetVendaRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var venda = await _context.Vendas
                .Where(v => v.EmpresaId == empresaId)
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            var usuario = await _context.Usuarios.FindAsync(new object[] { venda.UsuarioId }, cancellationToken);
            var vendedorRef = usuario != null ? new VendedorRefResponse { Id = usuario.Id, Nome = usuario.Nome ?? "" } : null;

            var itensResp = new List<ItemVendaResponse>();
            foreach (var i in venda.Itens)
            {
                var produto = await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == i.ProdutoId, cancellationToken);
                itensResp.Add(new ItemVendaResponse
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Produto = produto != null ? new ProdutoRefResponse
                    {
                        Id = produto.Id,
                        Nome = produto.Nome,
                        Sku = produto.SKU ?? "",
                        PrecoVenda = produto.PrecoVenda,
                        PrecoCusto = produto.PrecoCusto,
                        Categoria = produto.Categoria != null ? new CategoriaRefResponse { Id = produto.Categoria.Id, Nome = produto.Categoria.Nome } : null
                    } : null,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                });
            }
            return new GetVendaResponse
            {
                Id = venda.Id,
                CodigoVenda = venda.CodigoVenda,
                ClienteId = venda.ClienteId,
                Desconto = venda.Desconto,
                FormaPagamento = venda.FormaPagamento,
                UsuarioId = venda.UsuarioId,
                Vendedor = vendedorRef,
                Usuario = vendedorRef,
                DataHora = venda.DataHora,
                ValorTotal = venda.ValorTotal,
                Status = venda.Status,
                Itens = itensResp
            };
        }
    }
}
