using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.CriarUsuario
{
    public class CriarUsuarioRequest : IRequest<int>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        /// <summary>Senha em texto (o backend faz o hash). Aceita "senhaHash" ou "senha" no JSON.</summary>
        public string SenhaHash { get; set; } = string.Empty;
        /// <summary>Alternativa a SenhaHash - se frontend enviar "senha", usa aqui.</summary>
        public string? Senha { get; set; }
        public string Cargo { get; set; } = string.Empty;

        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public string? RG { get; set; }
        public string? Telefone { get; set; }
        public string? WhatsApp { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Perfil { get; set; }
        public string? Departamento { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; } = true;

        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }

        /// <summary>Ids das permissões adicionais (ex.: Gerenciar clientes, Acesso financeiro, Configurações do sistema).</summary>
        public List<int>? PermissaoIds { get; set; }

        /// <summary>Endereços do usuário (CEP, Rua, Número, Bairro, Cidade, etc.).</summary>
        public List<EnderecoRequest>? Enderecos { get; set; }
    }

    public class EnderecoRequest
    {
        public string Cep { get; set; } = string.Empty;
        /// <summary>Nome da rua/endereço. Aceita "rua" ou "endereco" no JSON.</summary>
        public string Rua { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string? Estado { get; set; }
    }
}
