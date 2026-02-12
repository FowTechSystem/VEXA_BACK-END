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
        var query = _context.Fornecedores
            .Include(f => f.Enderecos)  
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Valor))
        {
            if (request.Filtro?.ToLower() == "cnpj")
            {
                query = query.Where(c => c.CNPJ != null && c.CNPJ.Contains(request.Valor));
            }
            else
            {
                query = query.Where(c => (c.Nome != null && c.Nome.ToLower().Contains(request.Valor.ToLower())) ||
                                         (c.RazaoSocial != null && c.RazaoSocial.ToLower().Contains(request.Valor.ToLower())));
            }
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 50);

        var totalRegistros = await query.CountAsync(cancellationToken);
        var totalPaginas = totalRegistros > 0
            ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
            : 0;

        var fornecedores = await query
            .OrderBy(f => f.Nome)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return new GetFornecedorResponse
        {
            Fornecedor = fornecedores,
            PaginaAtual = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TamanhoPagina = tamanho
        };
    }
}
