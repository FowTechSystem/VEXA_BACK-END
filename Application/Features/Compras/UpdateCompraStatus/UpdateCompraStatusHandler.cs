using System.Security.Claims;
using CRMVEXA.Application.Services;
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
    private readonly IEntradaEstoquePorCompraService _entradaEstoque;

    public UpdateCompraStatusHandler(
        AppDbContext context,
        IEmpresaContext empresaContext,
        IHttpContextAccessor httpContextAccessor,
        IEntradaEstoquePorCompraService entradaEstoque)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
        _entradaEstoque = entradaEstoque;
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
            var jaEntrouEstoque = await _context.MovimentacoesEstoque
                .AnyAsync(m => m.CompraId == compra.Id, cancellationToken);

            if (!jaEntrouEstoque)
            {
                var usuarioId = ObterUsuarioIdDoToken();
                var motivo = $"Compra {compra.CodigoCompra} recebida";
                await _entradaEstoque.RegistrarEntradaAsync(empresaId, compra, usuarioId, motivo, cancellationToken);
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
