using System.Security.Claims;
using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Features.Vendas;
using CRMVEXA.Application.Features.Vendas.CreateVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
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

            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var venda = new Venda(empresaId, codigoVenda, usuarioId, formaPagamento)
                {
                    ClienteId = request.ClienteId,
                    Desconto = request.Desconto
                };
                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var itemRequest in request.Itens)
                {
                    var tipo = itemRequest.Tipo?.Trim().ToLowerInvariant() ?? "normal";
                    if (tipo == "bazar")
                    {
                        await ProcessarItemBazarAsync(empresaId, venda, itemRequest, codigoVenda, usuarioId, cancellationToken);
                        continue;
                    }

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
                    AdicionarItemVenda(venda, itemVenda);

                    produto.Estoque.AtualizarQuantidade(-itemRequest.Quantidade);
                    _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque(
                        empresaId,
                        itemRequest.ProdutoId,
                        "Saída",
                        itemRequest.Quantidade,
                        $"Venda {codigoVenda}",
                        usuarioId,
                        compraId: null,
                        vendaId: venda.Id));
                }

                await _context.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return await MontarRespostaAsync(venda, cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private void AdicionarItemVenda(Venda venda, ItemVenda item)
        {
            item.VendaId = venda.Id;
            venda.AdicionarItem(item);
            _context.ItensVenda.Add(item);
        }

        private async Task ProcessarItemBazarAsync(
            int empresaId,
            Venda venda,
            ItemVendaRequest itemRequest,
            string codigoVenda,
            int usuarioId,
            CancellationToken cancellationToken)
        {
            if (itemRequest.Quantidade != 1)
                throw BazarException.Validation("Peça de bazar sempre tem quantidade 1.");

            var hoje = DateTime.UtcNow.Date;
            var agora = DateTime.UtcNow;
            var atualizados = await _context.BazarProdutos
                .Where(p => p.Id == itemRequest.ProdutoId
                    && p.EmpresaId == empresaId
                    && p.DeletadoEm == null
                    && p.Status == BazarProdutoStatus.Disponivel)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, BazarProdutoStatus.Vendida)
                    .SetProperty(p => p.DataVenda, hoje)
                    .SetProperty(p => p.VendaId, venda.Id)
                    .SetProperty(p => p.AtualizadoEm, agora), cancellationToken);

            if (atualizados == 0)
            {
                var pecaInfo = await _context.BazarProdutos.AsNoTracking()
                    .Include(p => p.Consignante)
                    .FirstOrDefaultAsync(p => p.Id == itemRequest.ProdutoId && p.EmpresaId == empresaId, cancellationToken);
                if (pecaInfo == null)
                    throw new InvalidOperationException($"Peça de bazar com ID {itemRequest.ProdutoId} não encontrada");
                var nomeConsignante = pecaInfo.Consignante?.Nome ?? "consignante";
                throw BazarException.Conflict($"A peça '{pecaInfo.Descricao}' do consignante {nomeConsignante} já foi vendida ou devolvida.");
            }

            var itemVenda = new ItemVenda(itemRequest.ProdutoId, itemRequest.PrecoUnitario);
            AdicionarItemVenda(venda, itemVenda);
        }

        private async Task<CreateVendaResponse> MontarRespostaAsync(Venda venda, CancellationToken cancellationToken)
        {
            var itensDb = await _context.ItensVenda
                .Where(i => i.VendaId == venda.Id)
                .ToListAsync(cancellationToken);

            var itensMapeados = await VendaItemMapper.MapItensAsync(_context, _empresaContext.EmpresaId, itensDb, cancellationToken);
            var itensResponse = itensMapeados.Select(i => new CreateVendaItemResponse
            {
                Tipo = i.Tipo,
                ProdutoId = i.ProdutoId,
                BazarProdutoId = i.BazarProdutoId,
                ConsignanteNome = i.ConsignanteNome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Produto = i.Produto != null
                    ? new CreateVendaProdutoRefResponse { Id = i.Produto.Id, Nome = i.Produto.Nome }
                    : null
            }).ToList();

            var statusResposta = venda.Status?.Equals("Concluída", StringComparison.OrdinalIgnoreCase) == true
                ? "finalizada" : (venda.Status ?? "finalizada");

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
