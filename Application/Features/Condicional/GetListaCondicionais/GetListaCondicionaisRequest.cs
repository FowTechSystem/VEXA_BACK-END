using MediatR;

namespace CRMVEXA.Application.Features.Condicional.GetListaCondicionais;

public class GetListaCondicionaisRequest : IRequest<GetListaCondicionaisResponse>
{
    public bool? Pendente { get; set; }
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 100;
    public int? ClienteId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
