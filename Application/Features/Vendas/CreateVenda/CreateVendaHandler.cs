using CRMVEXA.Application.Features.Vendas.CreateVenda;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.CreateVenda
{
    public class CreateVendaHandler : IRequestHandler<CreateVendaRequest, int>
    {
        private readonly AppDbContext _context;

        public CreateVendaHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateVendaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var venda = new Venda(request.CodigoVenda, request.UsuarioId, request.FormaPagamento)
                {
                    ClienteId = request.ClienteId,
                    Desconto = request.Desconto
                };

                foreach (var itemRequest in request.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(new object[] { itemRequest.ProdutoId }, cancellationToken);
                    if (produto == null)
                    {
                        throw new Exception($"Produto com ID {itemRequest.ProdutoId} não encontrado");
                    }

                    if (produto.QuantidadeEstoque < itemRequest.Quantidade)
                    {
                        throw new Exception($"Estoque insuficiente para o produto {produto.Nome}. Disponível: {produto.QuantidadeEstoque}, Solicitado: {itemRequest.Quantidade}");
                    }

                    var itemVenda = new ItemVenda(itemRequest.ProdutoId, itemRequest.Quantidade, itemRequest.PrecoUnitario);
                    venda.AdicionarItem(itemVenda);

                    // Atualizar estoque
                    produto.QuantidadeEstoque -= itemRequest.Quantidade;

                    // Registrar movimentação de estoque
                    var movimentacao = new MovimentacaoEstoque(
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
