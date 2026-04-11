using MediatR;

namespace CRMVEXA.Application.Features.Caixa.ExcluirMovimentacaoManual;

public class ExcluirMovimentacaoManualRequest : IRequest<Unit>
{
    public int Id { get; set; }
}
