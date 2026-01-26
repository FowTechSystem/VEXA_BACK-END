namespace CRMVEXA.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string SenhaHash { get; set; }
        public string Cargo { get; set; }
        public bool PodeVender { get; set; }
        public bool PodeEditarEstoque { get; set; }
        public bool PodeEditarProduto { get; set; }
        public bool PodeAbrirFecharCaixa { get; set; }
        public bool PodeEmitirRelatorios { get; set; }
        public bool PodeCadastrarFornecedores { get; set; }

        private Usuario() { }

        public Usuario(string nome, string email, string login, string senhaHash, string cargo)
        {
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
