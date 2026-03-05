using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.GetListaClientes
{
    public class GetListaClientesHandler : IRequestHandler<GetListaClientesRequest, GetListaClientesResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetListaClientesHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetListaClientesResponse> Handle(GetListaClientesRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var query = _context.Clientes
                .Where(c => c.EmpresaId == empresaId)
                .Include(c => c.Enderecos)
                .AsQueryable();

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

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 500);

            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var clientes = await query
                .OrderBy(c => c.NomeCompleto)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);

            var clientesResp = clientes.Select(c => new ClienteListaResponse
            {
                Id = c.Id,
                Nome = c.NomeCompleto,
                Email = c.Email,
                DataCadastro = c.DataCadastro
            }).ToList();

            return new GetListaClientesResponse
            {
                Clientes = clientesResp,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = tamanho
            };
        }
    }
}
