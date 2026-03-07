using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Condicional.GetListaCondicionais;

public class GetListaCondicionaisHandler : IRequestHandler<GetListaCondicionaisRequest, GetListaCondicionaisResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaCondicionaisHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaCondicionaisResponse> Handle(GetListaCondicionaisRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível.");

        var empresaId = _empresaContext.EmpresaId;
        var query = _context.Condicionais
            .Where(c => c.EmpresaId == empresaId)
            .Include(c => c.Cliente)
            .Include(c => c.Itens)
            .AsQueryable();

        if (request.Pendente == true)
            query = query.Where(c => c.Status == "pendente");

        if (request.ClienteId.HasValue && request.ClienteId.Value > 0)
            query = query.Where(c => c.ClienteId == request.ClienteId.Value);

        if (request.DataInicio.HasValue)
        {
            var inicio = request.DataInicio.Value;
            if (inicio.Kind == DateTimeKind.Unspecified) inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
            query = query.Where(c => c.Data >= inicio);
        }
        if (request.DataFim.HasValue)
        {
            var fim = request.DataFim.Value.Date.AddDays(1);
            if (fim.Kind == DateTimeKind.Unspecified) fim = DateTime.SpecifyKind(fim, DateTimeKind.Utc);
            query = query.Where(c => c.Data < fim);
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 500);

        var condicionais = await query
            .OrderByDescending(c => c.Data)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        var produtoIds = condicionais.SelectMany(c => c.Itens.Select(i => i.ProdutoId)).Distinct().ToList();
        var produtos = produtoIds.Count > 0
            ? await _context.Produtos.Where(p => produtoIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.Produto>();

        var items = condicionais.Select(c => new CondicionalListaItem
        {
            Id = c.Id,
            Data = c.Data,
            ClienteId = c.ClienteId,
            ClienteNome = c.Cliente?.NomeCompleto,
            ValorTotal = c.ValorTotal,
            Status = c.Status,
            Itens = c.Itens.Select(i =>
            {
                produtos.TryGetValue(i.ProdutoId, out var p);
                return new CondicionalItemDto
                {
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = p?.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                };
            }).ToList()
        }).ToList();

        return new GetListaCondicionaisResponse { Condicionais = items };
    }
}
