using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Categorias.GetListaCategorias;

public class GetListaCategoriasHandler : IRequestHandler<GetListaCategoriasRequest, GetListaCategoriasResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaCategoriasHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaCategoriasResponse> Handle(GetListaCategoriasRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var categorias = await _context.Categorias
            .Where(c => c.EmpresaId == empresaId)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);

        return new GetListaCategoriasResponse
        {
            Categorias = categorias.Select(c => new CategoriaItemResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                Ativo = c.Ativo
            }).ToList()
        };
    }
}
