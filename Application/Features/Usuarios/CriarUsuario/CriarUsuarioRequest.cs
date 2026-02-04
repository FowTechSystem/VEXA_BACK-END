using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.CriarUsuario
{
    public class CriarUsuarioRequest : IRequest<int>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }
    }
}
