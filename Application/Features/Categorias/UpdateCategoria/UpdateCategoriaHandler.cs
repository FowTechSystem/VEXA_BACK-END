using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Categorias.UpdateCategoria;

public class UpdateCategoriaHandler : IRequestHandler<UpdateCategoriaRequest, UpdateCategoriaResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public UpdateCategoriaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<UpdateCategoriaResponse> Handle(UpdateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Id == request.Id, cancellationToken);

        if (categoria == null)
            return new UpdateCategoriaResponse { Sucesso = false, Mensagem = "Categoria não encontrada." };

        categoria.Nome = request.Nome;
        categoria.Descricao = request.Descricao;
        categoria.Ativo = request.Ativo;
        await _context.SaveChangesAsync(cancellationToken);
        return new UpdateCategoriaResponse { Sucesso = true, Mensagem = "Categoria atualizada com sucesso." };
    }
}
