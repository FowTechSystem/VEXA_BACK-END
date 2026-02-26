using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Categorias.CreateCategoria;

public class CreateCategoriaHandler : IRequestHandler<CreateCategoriaRequest, int>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public CreateCategoriaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<int> Handle(CreateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var categoria = new Categoria(request.Nome, request.Descricao)
        {
            EmpresaId = _empresaContext.EmpresaId
        };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync(cancellationToken);
        return categoria.Id;
    }
}
