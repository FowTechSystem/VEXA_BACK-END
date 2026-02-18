using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Empresas.CriarEmpresa
{
    public class CriarEmpresaHandler : IRequestHandler<CriarEmpresaRequest, CriarEmpresaResponse>
    {
        private readonly AppDbContext _context;

        public CriarEmpresaHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CriarEmpresaResponse> Handle(CriarEmpresaRequest request, CancellationToken cancellationToken)
        {
            var dataCadastro = DateTime.UtcNow;
            var empresa = new Empresa(request.Nome.Trim(), dataCadastro)
            {
                NomeFantasia = string.IsNullOrWhiteSpace(request.NomeFantasia) ? null : request.NomeFantasia.Trim(),
                CNPJ = string.IsNullOrWhiteSpace(request.Cnpj) ? null : request.Cnpj.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim(),
                Ativo = request.Ativo
            };

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync(cancellationToken);

            return new CriarEmpresaResponse
            {
                Id = empresa.Id,
                Nome = empresa.Nome,
                NomeFantasia = empresa.NomeFantasia,
                Cnpj = empresa.CNPJ,
                Email = empresa.Email,
                Telefone = empresa.Telefone,
                Ativo = empresa.Ativo,
                DataCadastro = empresa.DataCadastro
            };
        }
    }
}
