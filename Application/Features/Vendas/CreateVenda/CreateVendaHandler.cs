using System;
using CRMVEXA.Application.Features.Vendas.CreateVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.CreateVenda
{
    public class CreateVendaHandler : IRequestHandler<CreateVendaRequest, int>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public CreateVendaHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<int> Handle(CreateVendaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _empresaContext.EmpresaId;
                var venda = new Venda(empresaId, request.CodigoVenda, request.UsuarioId, request.FormaPagamento)
                {
                    ClienteId = request.ClienteId,
                    Desconto = request.Desconto
                };

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

                    var quantidadeDisponivel = produto.Estoque?.Quantidade ?? 0;
                    if (quantidadeDisponivel < itemRequest.Quantidade)
                    {
                        throw new Exception($"Estoque insuficiente para o produto {produto.Nome}. Disponível: {quantidadeDisponivel}, Solicitado: {itemRequest.Quantidade}");
                    }

                    var itemVenda = new ItemVenda(itemRequest.ProdutoId, itemRequest.Quantidade, itemRequest.PrecoUnitario);
                    venda.AdicionarItem(itemVenda);

                    // Atualizar estoque
                    if (produto.Estoque != null)
                        produto.Estoque.AtualizarQuantidade(-itemRequest.Quantidade);

                    // Registrar movimentação de estoque
                    var movimentacao = new MovimentacaoEstoque(
                        empresaId,
                        itemRequest.ProdutoId,
                        "Saída",
                        itemRequest.Quantidade,
                        $"Venda {request.CodigoVenda}",
                        request.UsuarioId
                    );
                    _context.MovimentacoesEstoque.Add(movimentacao);
                }

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync(cancellationToken);

                return venda.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar venda: {ex.Message}", ex);
            }
        }
    }
}
