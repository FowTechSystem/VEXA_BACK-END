using System.Security.Claims;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Application.Features.Compras.GetListaCompras;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.UpdateCompra;

public class UpdateCompraHandler : IRequestHandler<UpdateCompraRequest, UpdateCompraResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCompraHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UpdateCompraResponse> Handle(UpdateCompraRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;

        var compra = await _context.Compras
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Id == request.Id, cancellationToken);

        if (compra == null)
            return new UpdateCompraResponse { Sucesso = false, Mensagem = "Compra não encontrada" };

        var statusAtual = compra.Status?.Trim().ToLowerInvariant();
        if (statusAtual == "recebida")
            return new UpdateCompraResponse { Sucesso = false, Mensagem = "Compra já recebida não pode ser editada" };

        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.EmpresaId == empresaId && f.Id == request.FornecedorId, cancellationToken);
        if (fornecedor == null)
            return new UpdateCompraResponse { Sucesso = false, Mensagem = $"Fornecedor com ID {request.FornecedorId} não encontrado" };

        if (request.Itens == null || request.Itens.Count == 0)
            return new UpdateCompraResponse { Sucesso = false, Mensagem = "É necessário pelo menos um item na compra" };

        var usuarioId = request.UsuarioId ?? ObterUsuarioIdDoToken();
        if (usuarioId <= 0) usuarioId = compra.UsuarioId;

        foreach (var itemReq in request.Itens)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.EmpresaId == empresaId && p.Id == itemReq.ProdutoId, cancellationToken);
            if (produto == null)
                return new UpdateCompraResponse { Sucesso = false, Mensagem = $"Produto com ID {itemReq.ProdutoId} não encontrado" };
        }

        compra.FornecedorId = request.FornecedorId;
        compra.UsuarioId = usuarioId;
        compra.Observacoes = request.Observacoes;
        compra.DefinirStatusEDatas(
            NormalizarStatus(request.Status),
            request.DataPedido ?? compra.DataHora,
            request.DataEntregaPrevista,
            request.ValorTotal
        );

        _context.ItensCompra.RemoveRange(compra.Itens);

        foreach (var itemReq in request.Itens)
        {
            _context.ItensCompra.Add(new ItemCompra(itemReq.ProdutoId, itemReq.Quantidade, itemReq.PrecoUnitario)
            {
                CompraId = compra.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var compraAtualizada = await _context.Compras
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == compra.Id, cancellationToken);

        var response = await MontarGetCompraResponse(compraAtualizada!, cancellationToken);
        return new UpdateCompraResponse { Sucesso = true, Compra = response };
    }

    private static string NormalizarStatus(string? status)
    {
        var s = (status ?? "pendente").Trim().ToLowerInvariant();
        return s is "pendente" or "aprovada" or "recebida" or "cancelada" ? s : "pendente";
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }

    private async Task<GetCompraResponse> MontarGetCompraResponse(Domain.Entities.Compra compra, CancellationToken ct)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compra.FornecedorId }, ct);
        var usuario = await _context.Usuarios.FindAsync(new object[] { compra.UsuarioId }, ct);

        var resp = new GetCompraResponse
        {
            Id = compra.Id,
            CodigoCompra = compra.CodigoCompra,
            FornecedorId = compra.FornecedorId,
            Fornecedor = fornecedor != null
                ? new FornecedorRefResponse { Id = fornecedor.Id, Nome = fornecedor.Nome ?? "", RazaoSocial = fornecedor.RazaoSocial }
                : null,
            UsuarioId = compra.UsuarioId,
            UsuarioNome = usuario?.Nome,
            DataHora = compra.DataHora,
            DataEntregaPrevista = compra.DataEntregaPrevista,
            ValorTotal = compra.ValorTotal,
            Observacoes = compra.Observacoes,
            Status = compra.Status,
            Itens = new List<ItemCompraResponse>()
        };

        foreach (var item in compra.Itens)
        {
            var produto = await _context.Produtos.Include(p => p.Estoque).FirstOrDefaultAsync(p => p.Id == item.ProdutoId, ct);
            resp.Itens.Add(new ItemCompraResponse
            {
                Id = item.Id,
                CompraId = item.CompraId,
                ProdutoId = item.ProdutoId,
                ProdutoNome = produto?.Nome,
                Produto = produto != null ? new ProdutoRefResponse
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Sku = produto.SKU ?? "",
                    EstoqueMinimo = produto.Estoque?.EstoqueMinimo ?? 0,
                    EstoqueMaximo = 0,
                    PrecoCusto = produto.PrecoCusto,
                    PrecoVenda = produto.PrecoVenda
                } : null,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.Subtotal
            });
        }

        return resp;
    }
}
