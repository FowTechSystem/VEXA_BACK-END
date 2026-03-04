using MediatR;

namespace CRMVEXA.Application.Features.Estoque.GetListaEstoque;

public class GetListaEstoqueRequest : IRequest<GetListaEstoqueResponse>
{
    public string? Valor { get; set; }
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 50;
}
