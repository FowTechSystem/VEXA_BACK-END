using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.DeleteCatalogo;

public class DeleteCatalogoRequest : IRequest<bool>
{
    public int Id { get; set; }
}
