namespace CRMVEXA.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Login { get; private set; }
        public string SenhaHash { get; private set; }
        public string Cargo { get; private set; }
        public bool PodeVender { get; private set; }
        public bool PodeEditarEstoque { get; private set; }
        public bool PodeEditarProduto { get; private set; }
        public bool PodeAbrirFecharCaixa { get; private set; }
        public bool PodeEmitirRelatorios { get; private set; }
        public bool PodeCadastrarFornecedores { get; private set; }

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
