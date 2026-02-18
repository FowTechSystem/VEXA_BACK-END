using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Empresas.GetListaEmpresas
{
    public class GetListaEmpresasHandler : IRequestHandler<GetListaEmpresasRequest, GetListaEmpresasResponse>
    {
        private readonly AppDbContext _context;

        public GetListaEmpresasHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaEmpresasResponse> Handle(GetListaEmpresasRequest request, CancellationToken cancellationToken)
        {
            var empresas = await _context.Empresas
                .AsNoTracking()
                .OrderBy(e => e.Nome)
                .ToListAsync(cancellationToken);

            return new GetListaEmpresasResponse
            {
                Empresas = empresas.Select(e => new EmpresaResponse
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    NomeFantasia = e.NomeFantasia,
                    Cnpj = e.CNPJ,
                    Email = e.Email,
                    Telefone = e.Telefone,
                    Ativo = e.Ativo,
                    DataCadastro = e.DataCadastro
                }).ToList()
            };
        }
    }
}
