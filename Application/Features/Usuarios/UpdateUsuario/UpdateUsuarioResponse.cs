namespace CRMVEXA.Application.Features.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
    }
}
