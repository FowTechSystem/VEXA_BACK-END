using CRMVEXA.Application.Features.Clientes.GetListaClientes;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Fornecedor.GetFornecedor;

public class GetFornecedorHandler : IRequestHandler<GetFornecedorRequest, GetFornecedorResponse>
{
    private readonly AppDbContext _context;
    public GetFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetFornecedorResponse> Handle(GetFornecedorRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Fornecedores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Valor))
        {
            if (request.Filtro?.ToLower() == "cnpj")
            {
                query = query.Where(c => c.CNPJ != null && c.CNPJ.Contains(request.Valor));
            }
            else
            {
                query = query.Where(c => c.RazaoSocial.ToLower().Contains(request.Valor.ToLower()));
            }
        }
        var totalRegistros = await query.CountAsync(cancellationToken);
        var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)request.Tamanho);
        var fornecedores = await query
            .OrderBy(c => c.RazaoSocial)
            .Skip((request.Pagina - 1) * request.Tamanho)
            .Take(request.Tamanho)
            .ToListAsync(cancellationToken);
        return new GetFornecedorResponse
        {
            Fornecedor = fornecedores,
            PaginaAtual = request.Pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TamanhoPagina = request.Tamanho
        };
    }
}
