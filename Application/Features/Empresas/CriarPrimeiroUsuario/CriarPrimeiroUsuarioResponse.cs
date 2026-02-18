namespace CRMVEXA.Application.Features.Empresas.CriarPrimeiroUsuario
{
    public class CriarPrimeiroUsuarioResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
    }
}
