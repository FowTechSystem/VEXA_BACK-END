using MediatR;

namespace CRMVEXA.Application.Features.Marcas.GetMarca;

public class GetMarcaRequest : IRequest<GetMarcaResponse?>
{
    public int Id { get; set; }
}
