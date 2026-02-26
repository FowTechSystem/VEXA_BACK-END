using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Marcas.UpdateMarca;

public class UpdateMarcaHandler : IRequestHandler<UpdateMarcaRequest, UpdateMarcaResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public UpdateMarcaHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<UpdateMarcaResponse> Handle(UpdateMarcaRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var marca = await _context.Marcas
            .FirstOrDefaultAsync(m => m.EmpresaId == empresaId && m.Id == request.Id, cancellationToken);

        if (marca == null)
            return new UpdateMarcaResponse { Sucesso = false, Mensagem = "Marca não encontrada." };

        marca.Nome = request.Nome;
        marca.Descricao = request.Descricao;
        marca.Ativo = request.Ativo;
        await _context.SaveChangesAsync(cancellationToken);
        return new UpdateMarcaResponse { Sucesso = true, Mensagem = "Marca atualizada com sucesso." };
    }
}
