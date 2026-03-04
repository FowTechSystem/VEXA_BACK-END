using System.Security.Claims;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.UpdateCompraStatus;

public class UpdateCompraStatusHandler : IRequestHandler<UpdateCompraStatusRequest, UpdateCompraStatusResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCompraStatusHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UpdateCompraStatusResponse> Handle(UpdateCompraStatusRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var status = (request.Status ?? "").Trim().ToLowerInvariant();

        var compra = await _context.Compras
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Id == request.Id, cancellationToken);

        if (compra == null)
            return new UpdateCompraStatusResponse { Sucesso = false, Mensagem = "Compra não encontrada" };

        if (status != "pendente" && status != "aprovada" && status != "recebida" && status != "cancelada")
            return new UpdateCompraStatusResponse { Sucesso = false, Mensagem = "Status inválido. Use: pendente, aprovada, recebida ou cancelada" };

        if (compra.Status.ToLowerInvariant() == "recebida" && status == "recebida")
            return new UpdateCompraStatusResponse { Sucesso = true, Mensagem = "Compra já está com status recebida" };

        compra.Status = status;

        if (status == "recebida")
        {
            var usuarioId = ObterUsuarioIdDoToken();
            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos
                    .Where(p => p.EmpresaId == empresaId && p.Id == item.ProdutoId)
                    .Include(p => p.Estoque)
                    .FirstOrDefaultAsync(cancellationToken);

                if (produto == null) continue;

                if (produto.Estoque != null)
                    produto.Estoque.AtualizarQuantidade(item.Quantidade);
                else
                {
                    var estoque = new CRMVEXA.Domain.Entities.Estoque(empresaId, item.ProdutoId, item.Quantidade);
                    _context.Estoques.Add(estoque);
                }

                var movimentacao = new MovimentacaoEstoque(
                    empresaId,
                    item.ProdutoId,
                    "Entrada",
                    item.Quantidade,
                    $"Compra {compra.CodigoCompra} recebida",
                    usuarioId,
                    compraId: compra.Id
                );
                _context.MovimentacoesEstoque.Add(movimentacao);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return new UpdateCompraStatusResponse { Sucesso = true, Mensagem = $"Status atualizado para {status}" };
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
