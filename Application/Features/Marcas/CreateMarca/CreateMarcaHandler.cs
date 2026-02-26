using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Marcas.CreateMarca;

public class CreateMarcaHandler : IRequestHandler<CreateMarcaRequest, int>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public CreateMarcaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<int> Handle(CreateMarcaRequest request, CancellationToken cancellationToken)
    {
        var marca = new Marca(request.Nome, request.Descricao)
        {
            EmpresaId = _empresaContext.EmpresaId
        };
        _context.Marcas.Add(marca);
        await _context.SaveChangesAsync(cancellationToken);
        return marca.Id;
    }
}
