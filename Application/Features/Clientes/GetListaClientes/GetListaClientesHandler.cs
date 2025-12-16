using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.GetListaClientes.Validators
{
    public class GetListaClientesHandler : IRequestHandler<GetListaClientesRequest, GetListaClientesResponse>
    {
        private readonly AppDbContext _context;

        public GetListaClientesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaClientesResponse> Handle(GetListaClientesRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                if (request.Filtro?.ToLower() == "cpf")
                {
                    query = query.Where(c => c.CPF != null && c.CPF.Contains(request.Valor));
                }
                else 
                {
                    query = query.Where(c => c.NomeCompleto.ToLower().Contains(request.Valor.ToLower()));
                }
            }
            var totalRegistros = await query.CountAsync(cancellationToken);

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)request.Tamanho);

            var clientes = await query
                .OrderBy(c => c.NomeCompleto)
                .Skip((request.Pagina - 1) * request.Tamanho)
                .Take(request.Tamanho)
                .ToListAsync(cancellationToken);

            return new GetListaClientesResponse
            {
                Clientes = clientes,
                PaginaAtual = request.Pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = request.Tamanho
            };
        }
    }
}
