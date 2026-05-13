using CRMVEXA.Application.Abstractions;
using MediatR;

namespace CRMVEXA.Application.Features.Empresas.CriarEmpresa
{
    public class CriarEmpresaHandler : IRequestHandler<CriarEmpresaRequest, CriarEmpresaResponse>
    {
        private readonly ITenantProvisioningService _provisioning;

        public CriarEmpresaHandler(ITenantProvisioningService provisioning)
        {
            _provisioning = provisioning;
        }

        public async Task<CriarEmpresaResponse> Handle(CriarEmpresaRequest request, CancellationToken cancellationToken)
        {
            var id = await _provisioning.ProvisionAsync(
                request.Nome,
                request.NomeFantasia,
                request.Cnpj,
                request.Email,
                request.Telefone,
                request.Ativo,
                cancellationToken).ConfigureAwait(false);

            return new CriarEmpresaResponse
            {
                Id = id,
                Nome = request.Nome.Trim(),
                NomeFantasia = string.IsNullOrWhiteSpace(request.NomeFantasia) ? null : request.NomeFantasia.Trim(),
                Cnpj = string.IsNullOrWhiteSpace(request.Cnpj) ? null : request.Cnpj.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim(),
                Ativo = request.Ativo,
                DataCadastro = DateTime.UtcNow
            };
        }
    }
}
