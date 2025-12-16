namespace CRMVEXA.Application.Features.Clientes.UpdateCliente
{
    public class UpdateClienteResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
    }
}
