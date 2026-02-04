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
        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }
    }
}
