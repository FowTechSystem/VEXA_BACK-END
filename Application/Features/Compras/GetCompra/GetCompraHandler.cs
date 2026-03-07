using System;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Application.Features.Compras.GetListaCompras;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.GetCompra
{
    public class GetCompraHandler : IRequestHandler<GetCompraRequest, GetCompraResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetCompraHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetCompraResponse> Handle(GetCompraRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var compra = await _context.Compras
                .Where(c => c.EmpresaId == empresaId)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
                throw new KeyNotFoundException($"Compra com ID {request.Id} não encontrada");

            // Preencher dados relacionados
            var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compra.FornecedorId }, cancellationToken);
            var usuario = await _context.Usuarios.FindAsync(new object[] { compra.UsuarioId }, cancellationToken);

            var response = new GetCompraResponse
            {
                Id = compra.Id,
                CodigoCompra = compra.CodigoCompra,
                FornecedorId = compra.FornecedorId,
                Fornecedor = fornecedor != null ? new FornecedorRefResponse { Id = fornecedor.Id, Nome = fornecedor.Nome ?? "", RazaoSocial = fornecedor.RazaoSocial } : null,
                UsuarioId = compra.UsuarioId,
                UsuarioNome = usuario?.Nome,
                DataHora = compra.DataHora,
                DataEntregaPrevista = compra.DataEntregaPrevista,
                ValorTotal = compra.ValorTotal,
                Observacoes = compra.Observacoes,
                Status = compra.Status,
                Itens = new List<ItemCompraResponse>()
            };

            // Preencher itens com nomes dos produtos
            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos.Include(p => p.Estoque).FirstOrDefaultAsync(p => p.Id == item.ProdutoId, cancellationToken);
                response.Itens.Add(new ItemCompraResponse
                {
                    Id = item.Id,
                    CompraId = item.CompraId,
                    ProdutoId = item.ProdutoId,
                    ProdutoNome = produto?.Nome,
                    Produto = produto != null ? new ProdutoRefResponse
                    {
                        Id = produto.Id,
                        Nome = produto.Nome,
                        Sku = produto.SKU ?? "",
                        EstoqueMinimo = produto.Estoque?.EstoqueMinimo ?? 0,
                        EstoqueMaximo = 0,
                        PrecoCusto = produto.PrecoCusto,
                        PrecoVenda = produto.PrecoVenda
                    } : null,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario,
                    Subtotal = item.Subtotal
                });
            }

            return response;
        }
    }
}
