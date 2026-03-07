using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Condicional.GetCondicional;

public class GetCondicionalHandler : IRequestHandler<GetCondicionalRequest, GetCondicionalResponse?>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetCondicionalHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetCondicionalResponse?> Handle(GetCondicionalRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível.");

        var empresaId = _empresaContext.EmpresaId;
        var condicional = await _context.Condicionais
            .Where(c => c.EmpresaId == empresaId && c.Id == request.Id)
            .Include(c => c.Cliente)
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(cancellationToken);

        if (condicional == null)
            return null;

        var produtoIds = condicional.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = produtoIds.Count > 0
            ? await _context.Produtos.Where(p => produtoIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.Produto>();

        return new GetCondicionalResponse
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
                return new GetCondicionalItemResponse
                {
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = p?.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                };
            }).ToList()
        };
    }
}
