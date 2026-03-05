namespace CRMVEXA.Application.Features.Clientes.GetListaClientes;

public class ClienteListaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataCriacao { get => DataCadastro; set => DataCadastro = value; }
}
