namespace CRMVEXA.Application.Features.Usuarios.GetListaUsuarios
{
    public class GetListaUsuariosResponse
    {
        public List<UsuarioResponse> Usuarios { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanhoPagina { get; set; }
    }

    /// <summary>
    /// Resposta sem SenhaHash por segurança.
    /// </summary>
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
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
        public bool Ativo { get; set; }
        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }
        public List<EnderecoResponse> Enderecos { get; set; } = new();
        public List<int> PermissaoIds { get; set; } = new();
    }

    public class EnderecoResponse
    {
        public int Id { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string? Estado { get; set; }
    }
}
