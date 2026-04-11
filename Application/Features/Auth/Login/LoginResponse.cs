namespace CRMVEXA.Application.Features.Auth.Login
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public LoginUserResponse? User { get; set; }
    }

    public class LoginUserResponse
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        /// <summary>Nome da empresa (para exibir no header/sidebar do front).</summary>
        public string EmpresaNome { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty; // Cargo no sistema (ex: Administrador)
        public string? FotoUrl { get; set; }
    }
}
