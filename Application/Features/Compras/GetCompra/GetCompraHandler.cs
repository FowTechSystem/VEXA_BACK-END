using System;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.GetCompra
{
    public class GetCompraHandler : IRequestHandler<GetCompraRequest, GetCompraResponse>
    {
        private readonly AppDbContext _context;

        public GetCompraHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetCompraResponse> Handle(GetCompraRequest request, CancellationToken cancellationToken)
        {
            var compra = await _context.Compras
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
            {
                throw new Exception($"Compra com ID {request.Id} não encontrada");
            }

            // Preencher dados relacionados
            var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compra.FornecedorId }, cancellationToken);
            var usuario = await _context.Usuarios.FindAsync(new object[] { compra.UsuarioId }, cancellationToken);

            var response = new GetCompraResponse
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

            // Preencher itens com nomes dos produtos
            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos.FindAsync(new object[] { item.ProdutoId }, cancellationToken);
                response.Itens.Add(new ItemCompraResponse
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

            return response;
        }
    }
}
