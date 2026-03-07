using MediatR;

namespace CRMVEXA.Application.Features.Condicional.DevolverCondicional;

public class DevolverCondicionalRequest : IRequest<DevolverCondicionalResponse>
{
    public int Id { get; set; }
}
