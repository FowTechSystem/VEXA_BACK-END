using MediatR;

namespace CRMVEXA.Application.Features.Dashboard.GetDashboardData;

public class GetDashboardDataRequest : IRequest<GetDashboardDataResponse>
{
    public int Mes { get; set; }
    public int Ano { get; set; }
}
