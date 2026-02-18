using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioRequest : IRequest<UpdateUsuarioResponse>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;

        public string? CPF { get; set; }
        public string? RG { get; set; }
        public string? Telefone { get; set; }
        public string? WhatsApp { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Perfil { get; set; }
        public string? Departamento { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; } = true;

        /// <summary>Nova senha (opcional). Se preenchido, atualiza a senha com hash BCrypt.</summary>
        public string? NovaSenha { get; set; }

        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }

        public List<int>? PermissaoIds { get; set; }
        public List<EnderecoRequest>? Enderecos { get; set; }
    }

    public class EnderecoRequest
    {
        public int? Id { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string? Estado { get; set; }
    }
}
