using CRMVEXA.Application.Features.Clientes;
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
                var valor = request.Valor.Trim();
                var valorLower = valor.ToLower();
                var f = request.Filtro?.Trim().ToLowerInvariant();
                if (f == "cpf")
                    query = query.Where(c => c.CPF != null && c.CPF.Contains(valor));
                else if (f == "email")
                    query = query.Where(c => c.Email != null && c.Email.ToLower().Contains(valorLower));
                else
                    query = query.Where(c => c.NomeCompleto.ToLower().Contains(valorLower));
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
                NomeCompleto = c.NomeCompleto,
                Email = c.Email,
                CPF = c.CPF,
                RG = c.RG,
                CNPJ = c.CNPJ,
                DataNascimento = c.DataNascimento,
                Telefone = c.Telefone,
                WhatsApp = c.WhatsApp,
                Preferencias = c.Preferencias,
                Observacoes = c.Observacoes,
                FotoUrl = ClienteFotoUrlHelper.ToApiFotoUrl(c.Foto),
                DataCadastro = c.DataCadastro,
                Enderecos = c.Enderecos?.Select(e => new EnderecoClienteResponse
                {
                    Rua = e.Rua,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Cep = e.Cep
                }).ToList() ?? new List<EnderecoClienteResponse>()
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
