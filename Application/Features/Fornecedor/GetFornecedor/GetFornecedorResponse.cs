using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Features.Fornecedor.GetFornecedor;

public class GetFornecedorResponse
{
    public List<CRMVEXA.Domain.Entities.Fornecedor> Fornecedor { get; set; } = new();
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int TamanhoPagina { get; set; }
}
