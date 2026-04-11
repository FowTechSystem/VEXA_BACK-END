using System;
using System.Security.Claims;
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
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            if (venda.Status == "Cancelada")
            {
                throw new Exception("Venda já está cancelada");
            }

            var usuarioIdMov = request.UsuarioId > 0
                ? request.UsuarioId
                : ObterUsuarioIdDoToken();
            if (usuarioIdMov <= 0)
                usuarioIdMov = venda.UsuarioId;

            // Restaurar estoque
            foreach (var item in venda.Itens)
            {
                var produto = await _context.Produtos
                    .Include(p => p.Estoque)
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId, cancellationToken);
                if (produto != null && produto.Estoque != null)
                {
                    produto.Estoque.AtualizarQuantidade(item.Quantidade);

                    // Registrar movimentação de estoque
                    var motivoTxt = string.IsNullOrWhiteSpace(request.Motivo)
                        ? $"Cancelamento da venda {venda.CodigoVenda}"
                        : $"Cancelamento da venda {venda.CodigoVenda}: {request.Motivo.Trim()}";
                    var movimentacao = new MovimentacaoEstoque(
                        empresaId,
                        item.ProdutoId,
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
