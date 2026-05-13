using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Empresas.GetListaEmpresas
{
    public class GetListaEmpresasHandler : IRequestHandler<GetListaEmpresasRequest, GetListaEmpresasResponse>
    {
        private readonly MasterDbContext _master;

        public GetListaEmpresasHandler(MasterDbContext master)
        {
            _master = master;
        }

        public async Task<GetListaEmpresasResponse> Handle(GetListaEmpresasRequest request, CancellationToken cancellationToken)
        {
            var empresas = await _master.Tenants
                .AsNoTracking()
                .OrderBy(e => e.Nome)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new GetListaEmpresasResponse
            {
                Empresas = empresas.Select(e => new EmpresaResponse
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    NomeFantasia = e.NomeFantasia,
                    Cnpj = e.Cnpj,
                    Email = e.Email,
                    Telefone = e.Telefone,
                    Ativo = e.Ativo,
                    DataCadastro = e.DataCadastro
                }).ToList()
            };
        }
    }
}
