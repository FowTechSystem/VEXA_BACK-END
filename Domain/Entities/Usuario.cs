namespace CRMVEXA.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string SenhaHash { get; set; }
        public string Cargo { get; set; }

        // Campos do formulário (Geral / Extras)
        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public string? RG { get; set; }
        public string? Telefone { get; set; }
        public string? WhatsApp { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Perfil { get; set; }       // ex: Gerente, Administrador
        public string? Departamento { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; } = true;

        // Permissões em booleans (mantidas para compatibilidade)
        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }

        // Permissões adicionais via entidade Permissao (ex.: Gerenciar clientes, Acesso financeiro, Configurações do sistema)
        public ICollection<UsuarioPermissao> UsuarioPermissoes { get; set; } = new List<UsuarioPermissao>();

        // Endereços do usuário
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

        public Empresa Empresa { get; set; } = null!;

        private Usuario() { }

        public Usuario(int empresaId, string nome, string email, string login, string senhaHash, string cargo)
        {
            EmpresaId = empresaId;
            Nome = nome;
            Email = email;
            Login = login;
            SenhaHash = senhaHash;
            Cargo = cargo;
        }

        public void DefinirPermissoes(
            bool podeVender,
            bool podeEditarEstoque,
            bool podeEditarProduto,
            bool podeAbrirFecharCaixa,
            bool podeEmitirRelatorios,
            bool podeCadastrarFornecedores)
        {
            PodeVender = podeVender;
            PodeEditarEstoque = podeEditarEstoque;
            PodeEditarProduto = podeEditarProduto;
            PodeAbrirFecharCaixa = podeAbrirFecharCaixa;
            PodeEmitirRelatorios = podeEmitirRelatorios;
            PodeCadastrarFornecedores = podeCadastrarFornecedores;
        }
    }
}
