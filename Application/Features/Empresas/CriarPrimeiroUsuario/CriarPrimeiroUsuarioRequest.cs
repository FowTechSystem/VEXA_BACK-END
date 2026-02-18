using MediatR;

namespace CRMVEXA.Application.Features.Empresas.CriarPrimeiroUsuario
{
    /// <summary>
    /// Cria o primeiro usuário de uma empresa (apenas para dev@gmail.com).
    /// </summary>
    public class CriarPrimeiroUsuarioRequest : IRequest<CriarPrimeiroUsuarioResponse>
    {
        public int EmpresaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
