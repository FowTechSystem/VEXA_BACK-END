using MediatR;

namespace CRMVEXA.Application.Features.Caixa.GetResumo;

public class GetCaixaResumoRequest : IRequest<GetCaixaResumoResponse>
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
