using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Categorias.DeleteCategoria;

public class DeleteCategoriaHandler : IRequestHandler<DeleteCategoriaRequest, DeleteCategoriaResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public DeleteCategoriaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<DeleteCategoriaResponse> Handle(DeleteCategoriaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var categoria = await _context.Categorias
            .Include(c => c.Produtos)
            .Include(c => c.Subcategorias)
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Id == request.Id, cancellationToken);

        if (categoria == null)
            return new DeleteCategoriaResponse { Sucesso = false, Mensagem = "Categoria não encontrada." };

        if (categoria.Produtos.Any())
            return new DeleteCategoriaResponse { Sucesso = false, Mensagem = "Não é possível excluir: existem produtos vinculados a esta categoria." };

        if (categoria.Subcategorias.Any())
            return new DeleteCategoriaResponse { Sucesso = false, Mensagem = "Não é possível excluir: existem subcategorias vinculadas a esta categoria." };

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync(cancellationToken);
        return new DeleteCategoriaResponse { Sucesso = true, Mensagem = "Categoria excluída com sucesso." };
    }
}
