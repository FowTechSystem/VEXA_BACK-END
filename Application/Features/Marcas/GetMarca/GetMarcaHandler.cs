using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Marcas.GetMarca;

public class GetMarcaHandler : IRequestHandler<GetMarcaRequest, GetMarcaResponse?>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetMarcaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetMarcaResponse?> Handle(GetMarcaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var marca = await _context.Marcas
            .Where(m => m.EmpresaId == empresaId && m.Id == request.Id)
            .Select(m => new GetMarcaResponse
            {
                Id = m.Id,
                Nome = m.Nome,
                Descricao = m.Descricao,
                Ativo = m.Ativo,
                DataCadastro = m.DataCadastro
            })
            .FirstOrDefaultAsync(cancellationToken);
        return marca;
    }
}
