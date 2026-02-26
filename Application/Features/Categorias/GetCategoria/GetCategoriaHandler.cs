using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Categorias.GetCategoria;

public class GetCategoriaHandler : IRequestHandler<GetCategoriaRequest, GetCategoriaResponse?>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetCategoriaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetCategoriaResponse?> Handle(GetCategoriaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var categoria = await _context.Categorias
            .Where(c => c.EmpresaId == empresaId && c.Id == request.Id)
            .Select(c => new GetCategoriaResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                Ativo = c.Ativo,
                DataCadastro = c.DataCadastro
            })
            .FirstOrDefaultAsync(cancellationToken);
        return categoria;
    }
}
