using System.Security.Claims;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Condicional.DevolverCondicional;

public class DevolverCondicionalHandler : IRequestHandler<DevolverCondicionalRequest, DevolverCondicionalResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DevolverCondicionalHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DevolverCondicionalResponse> Handle(DevolverCondicionalRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível.");

        var empresaId = _empresaContext.EmpresaId;
        var usuarioId = ObterUsuarioIdDoToken();

        var condicional = await _context.Condicionais
            .Where(c => c.EmpresaId == empresaId && c.Id == request.Id)
            .Include(c => c.Cliente)
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(cancellationToken);

        if (condicional == null)
            throw new InvalidOperationException($"Condicional com ID {request.Id} não encontrado.");

        if (condicional.Status == "devolvido")
            throw new InvalidOperationException("Este condicional já foi devolvido.");

        foreach (var item in condicional.Itens)
        {
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId && p.Id == item.ProdutoId)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(cancellationToken);
            if (produto != null && produto.Estoque != null)
            {
                produto.Estoque.AtualizarQuantidade(item.Quantidade);
                var movimentacao = new MovimentacaoEstoque(
                    empresaId,
                    item.ProdutoId,
                    "Entrada",
                    item.Quantidade,
                    "Devolução Condicional",
                    usuarioId,
                    compraId: null,
                    vendaId: null);
                _context.MovimentacoesEstoque.Add(movimentacao);
            }
        }

        condicional.MarcarDevolvido();
        await _context.SaveChangesAsync(cancellationToken);

        var produtoIds = condicional.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = produtoIds.Count > 0
            ? await _context.Produtos.Where(p => produtoIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.Produto>();

        return new DevolverCondicionalResponse
        {
            Id = condicional.Id,
            Data = condicional.Data,
            ClienteId = condicional.ClienteId,
            ClienteNome = condicional.Cliente?.NomeCompleto,
            ValorTotal = condicional.ValorTotal,
            Status = condicional.Status,
            Itens = condicional.Itens.Select(i =>
            {
                produtos.TryGetValue(i.ProdutoId, out var p);
                return new DevolverCondicionalItemResponse
                {
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = p?.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                };
            }).ToList()
        };
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
