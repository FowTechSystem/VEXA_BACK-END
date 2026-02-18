using MediatR;

namespace CRMVEXA.Application.Features.Empresas.CriarEmpresa
{
    public class CriarEmpresaRequest : IRequest<CriarEmpresaResponse>
    {
        public string Nome { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string? Cnpj { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
