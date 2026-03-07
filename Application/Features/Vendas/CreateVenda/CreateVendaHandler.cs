using System.Security.Claims;
using CRMVEXA.Application.Features.Vendas.CreateVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.CreateVenda
{
    public class CreateVendaHandler : IRequestHandler<CreateVendaRequest, CreateVendaResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateVendaHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _empresaContext = empresaContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateVendaResponse> Handle(CreateVendaRequest request, CancellationToken cancellationToken)
        {
            if (!_empresaContext.TemEmpresa)
                throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

            var empresaId = _empresaContext.EmpresaId;
            var usuarioId = request.UsuarioId ?? ObterUsuarioIdDoToken();
            if (usuarioId <= 0)
                throw new InvalidOperationException("Usuário não identificado. Faça login novamente.");

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId && u.EmpresaId == empresaId, cancellationToken);
            if (!usuarioExiste)
                throw new InvalidOperationException($"Usuário com ID {usuarioId} não encontrado para esta empresa.");

            if (request.ClienteId.HasValue && request.ClienteId.Value > 0)
            {
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == request.ClienteId.Value && c.EmpresaId == empresaId, cancellationToken);
                if (!clienteExiste)
                    throw new InvalidOperationException($"Cliente com ID {request.ClienteId} não encontrado para esta empresa.");
            }

            if (request.Itens == null || request.Itens.Count == 0)
                throw new InvalidOperationException("A venda deve ter pelo menos um item.");

            var codigoVenda = !string.IsNullOrWhiteSpace(request.CodigoVenda)
                ? request.CodigoVenda.Trim()
                : $"VND-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
            var formaPagamento = NormalizarFormaPagamento(request.FormaPagamento);

            var venda = new Venda(empresaId, codigoVenda, usuarioId, formaPagamento)
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
                    throw new InvalidOperationException($"Produto com ID {itemRequest.ProdutoId} não encontrado");

                if (produto.Estoque == null)
                    throw new InvalidOperationException($"Produto '{produto.Nome}' (ID {produto.Id}) não possui registro de estoque. Cadastre o estoque antes de vender.");

                var quantidadeDisponivel = produto.Estoque.Quantidade;
                if (quantidadeDisponivel < itemRequest.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Nome}. Disponível: {quantidadeDisponivel}, Solicitado: {itemRequest.Quantidade}");

                var itemVenda = new ItemVenda(itemRequest.ProdutoId, itemRequest.Quantidade, itemRequest.PrecoUnitario);
                venda.AdicionarItem(itemVenda);

                produto.Estoque.AtualizarQuantidade(-itemRequest.Quantidade);

                var movimentacao = new MovimentacaoEstoque(
                    empresaId,
                    itemRequest.ProdutoId,
                    "Saída",
                    itemRequest.Quantidade,
                    $"Venda {codigoVenda}",
                    usuarioId,
                    compraId: null,
                    vendaId: null);
                _context.MovimentacoesEstoque.Add(movimentacao);
            }

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync(cancellationToken);

            var itensResponse = new List<CreateVendaItemResponse>();
            foreach (var i in venda.Itens)
            {
                var p = await _context.Produtos.FindAsync(new object[] { i.ProdutoId }, cancellationToken);
                itensResponse.Add(new CreateVendaItemResponse
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Produto = p != null ? new CreateVendaProdutoRefResponse { Id = p.Id, Nome = p.Nome } : null
                });
            }

            var statusResposta = venda.Status?.Equals("Concluída", StringComparison.OrdinalIgnoreCase) == true ? "finalizada" : (venda.Status ?? "finalizada");

            return new CreateVendaResponse
            {
                Id = venda.Id,
                DataVenda = venda.DataHora,
                ValorTotal = venda.ValorTotal,
                FormaPagamento = venda.FormaPagamento,
                Desconto = venda.Desconto ?? 0,
                Status = statusResposta,
                Itens = itensResponse
            };
        }

        private static string NormalizarFormaPagamento(string forma)
        {
            if (string.IsNullOrWhiteSpace(forma)) return "Dinheiro";
            var f = forma.Trim().ToLowerInvariant();
            if (f is "credito" or "crédito" or "cartaocredito") return "CartaoCredito";
            if (f is "debito" or "débito" or "cartaodebito") return "CartaoDebito";
            if (f == "pix") return "Pix";
            if (f == "dinheiro") return "Dinheiro";
            if (f is "crediario" or "crediário") return "Crediario";
            return forma;
        }

        private int ObterUsuarioIdDoToken()
        {
            var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
