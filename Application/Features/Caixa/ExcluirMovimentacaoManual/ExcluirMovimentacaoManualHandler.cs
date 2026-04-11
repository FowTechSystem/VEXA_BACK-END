using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Caixa.ExcluirMovimentacaoManual;

public class ExcluirMovimentacaoManualHandler : IRequestHandler<ExcluirMovimentacaoManualRequest, Unit>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public ExcluirMovimentacaoManualHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<Unit> Handle(ExcluirMovimentacaoManualRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;
        var mov = await _context.MovimentacoesCaixaManuais
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.EmpresaId == empresaId, cancellationToken);

        if (mov == null)
            throw new KeyNotFoundException("Movimentação manual não encontrada.");

        _context.MovimentacoesCaixaManuais.Remove(mov);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
