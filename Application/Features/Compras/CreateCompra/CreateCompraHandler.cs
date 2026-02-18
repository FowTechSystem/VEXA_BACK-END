using System;
using CRMVEXA.Application.Features.Compras.CreateCompra;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.CreateCompra
{
    public class CreateCompraHandler : IRequestHandler<CreateCompraRequest, int>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public CreateCompraHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<int> Handle(CreateCompraRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _empresaContext.EmpresaId;
                var compra = new Compra(empresaId, request.CodigoCompra, request.FornecedorId, request.UsuarioId, request.Observacoes);

                foreach (var itemRequest in request.Itens)
                {
                    var produto = await _context.Produtos
                        .Where(p => p.EmpresaId == empresaId)
                        .Include(p => p.Estoque)
                        .FirstOrDefaultAsync(p => p.Id == itemRequest.ProdutoId, cancellationToken);
                    if (produto == null)
                    {
                        throw new Exception($"Produto com ID {itemRequest.ProdutoId} não encontrado");
                    }

                    var itemCompra = new ItemCompra(itemRequest.ProdutoId, itemRequest.Quantidade, itemRequest.PrecoUnitario);
                    compra.AdicionarItem(itemCompra);

                    // Atualizar estoque
                    if (produto.Estoque != null)
                        produto.Estoque.AtualizarQuantidade(itemRequest.Quantidade);

                    // Registrar movimentação de estoque
                    var movimentacao = new MovimentacaoEstoque(
                        empresaId,
                        itemRequest.ProdutoId,
                        "Entrada",
                        itemRequest.Quantidade,
                        $"Compra {request.CodigoCompra}",
                        request.UsuarioId
                    );
                    _context.MovimentacoesEstoque.Add(movimentacao);
                }

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync(cancellationToken);

                return compra.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar compra: {ex.Message}", ex);
            }
        }
    }
}
