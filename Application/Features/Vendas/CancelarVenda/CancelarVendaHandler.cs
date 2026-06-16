using System;
using System.Security.Claims;
using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Features.Vendas.CancelarVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.CancelarVenda
{
    public class CancelarVendaHandler : IRequestHandler<CancelarVendaRequest, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CancelarVendaHandler(
            AppDbContext context,
            IEmpresaContext empresaContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _empresaContext = empresaContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(CancelarVendaRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var venda = await _context.Vendas
                .Where(v => v.EmpresaId == empresaId)
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new InvalidOperationException($"Venda com ID {request.Id} não encontrada");
            }

            if (venda.Status == "Cancelada")
            {
                throw new InvalidOperationException("Venda já está cancelada");
            }

            var usuarioIdMov = request.UsuarioId > 0
                ? request.UsuarioId
                : ObterUsuarioIdDoToken();
            if (usuarioIdMov <= 0)
                usuarioIdMov = venda.UsuarioId;

            foreach (var item in venda.Itens)
            {
                if (string.Equals(item.TipoItem, "bazar", StringComparison.OrdinalIgnoreCase) && item.BazarProdutoId.HasValue)
                {
                    var pecaBazar = await _context.BazarProdutos
                        .FirstOrDefaultAsync(p => p.Id == item.BazarProdutoId.Value && p.EmpresaId == empresaId, cancellationToken);
                    if (pecaBazar != null && pecaBazar.VendaId == venda.Id)
                    {
                        var emRepasse = await _context.BazarRepasseItens.AnyAsync(ri =>
                            ri.ProdutoId == pecaBazar.Id &&
                            _context.BazarRepasses.Any(r => r.Id == ri.RepasseId && r.DeletadoEm == null),
                            cancellationToken);
                        if (emRepasse)
                        {
                            throw BazarException.Conflict(
                                $"A peça '{pecaBazar.Descricao}' já entrou em um repasse ao consignante. " +
                                "Cancele o repasse antes de cancelar esta venda.");
                        }

                        pecaBazar.Status = Domain.Enums.BazarProdutoStatus.Disponivel;
                        pecaBazar.DataVenda = null;
                        pecaBazar.VendaId = null;
                        pecaBazar.AtualizadoEm = DateTime.UtcNow;
                    }
                    continue;
                }

                if (!item.ProdutoId.HasValue) continue;

                var produto = await _context.Produtos
                    .Include(p => p.Estoque)
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId.Value, cancellationToken);
                if (produto != null && produto.Estoque != null)
                {
                    produto.Estoque.AtualizarQuantidade(item.Quantidade);

                    var motivoTxt = string.IsNullOrWhiteSpace(request.Motivo)
                        ? $"Cancelamento da venda {venda.CodigoVenda}"
                        : $"Cancelamento da venda {venda.CodigoVenda}: {request.Motivo.Trim()}";
                    var movimentacao = new MovimentacaoEstoque(
                        empresaId,
                        item.ProdutoId.Value,
                        "Entrada",
                        item.Quantidade,
                        motivoTxt,
                        usuarioIdMov
                    );
                    _context.MovimentacoesEstoque.Add(movimentacao);
                }
            }

            venda.Cancelar();
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private int ObterUsuarioIdDoToken()
        {
            var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
