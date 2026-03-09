using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Catalogo.GetListaCatalogos;

public class GetListaCatalogosHandler : IRequestHandler<GetListaCatalogosRequest, GetListaCatalogosResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetListaCatalogosHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetListaCatalogosResponse> Handle(GetListaCatalogosRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível.");

        var empresaId = _empresaContext.EmpresaId;
        var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" +
                      _httpContextAccessor.HttpContext?.Request.Host;

        var query = _context.Catalogos
            .Where(c => c.EmpresaId == empresaId)
            .AsQueryable();

        var total = await query.CountAsync(cancellationToken);
        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 100);

        var catalogos = await query
            .OrderByDescending(c => c.CriadoEm)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(c => new
            {
                c.Id,
                c.EmpresaNome,
                c.CriadoEm,
                c.Ativo,
                QtdProdutos = c.Produtos.Count
            })
            .ToListAsync(cancellationToken);

        var items = catalogos.Select(c => new CatalogoListaItem
        {
            Id = c.Id,
            EmpresaNome = c.EmpresaNome,
            LinkPublico = $"{baseUrl}/catalogo-publico/{c.Id}",
            QuantidadeProdutos = c.QtdProdutos,
            CriadoEm = c.CriadoEm,
            Ativo = c.Ativo
        }).ToList();

        return new GetListaCatalogosResponse
        {
            Items = items,
            Total = total,
            Pagina = pagina
        };
    }
}
