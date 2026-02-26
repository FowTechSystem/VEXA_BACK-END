using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Marcas.GetListaMarcas;

public class GetListaMarcasHandler : IRequestHandler<GetListaMarcasRequest, GetListaMarcasResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaMarcasHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaMarcasResponse> Handle(GetListaMarcasRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var marcas = await _context.Marcas
            .Where(m => m.EmpresaId == empresaId)
            .OrderBy(m => m.Nome)
            .ToListAsync(cancellationToken);

        return new GetListaMarcasResponse
        {
            Marcas = marcas.Select(m => new MarcaItemResponse
            {
                Id = m.Id,
                Nome = m.Nome,
                Descricao = m.Descricao,
                Ativo = m.Ativo
            }).ToList()
        };
    }
}
