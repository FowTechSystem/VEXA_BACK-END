using MediatR;

namespace CRMVEXA.Application.Features.Estoque.GetListaMovimentacoes;

public class GetListaMovimentacoesRequest : IRequest<GetListaMovimentacoesResponse>
{
    public int? ProdutoId { get; set; }
    public string? Tipo { get; set; }
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 50;
}
