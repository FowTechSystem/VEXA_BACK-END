using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Marcas.DeleteMarca;

public class DeleteMarcaHandler : IRequestHandler<DeleteMarcaRequest, DeleteMarcaResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public DeleteMarcaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<DeleteMarcaResponse> Handle(DeleteMarcaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var marca = await _context.Marcas
            .Include(m => m.Produtos)
            .FirstOrDefaultAsync(m => m.EmpresaId == empresaId && m.Id == request.Id, cancellationToken);

        if (marca == null)
            return new DeleteMarcaResponse { Sucesso = false, Mensagem = "Marca não encontrada." };

        if (marca.Produtos.Any())
            return new DeleteMarcaResponse { Sucesso = false, Mensagem = "Não é possível excluir: existem produtos vinculados a esta marca." };

        _context.Marcas.Remove(marca);
        await _context.SaveChangesAsync(cancellationToken);
        return new DeleteMarcaResponse { Sucesso = true, Mensagem = "Marca excluída com sucesso." };
    }
}
