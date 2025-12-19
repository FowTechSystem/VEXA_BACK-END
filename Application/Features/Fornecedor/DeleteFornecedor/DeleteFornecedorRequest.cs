using MediatR;

namespace CRMVEXA.Application.Features.Fornecedor.DeleteFornecedor;

public class DeleteFornecedorRequest :IRequest<bool>
{
    public int id { get; set; }
}
