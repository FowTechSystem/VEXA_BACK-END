using MediatR;

namespace CRMVEXA.Application.Features.Condicional.GetCondicional;

public class GetCondicionalRequest : IRequest<GetCondicionalResponse?>
{
    public int Id { get; set; }
}
