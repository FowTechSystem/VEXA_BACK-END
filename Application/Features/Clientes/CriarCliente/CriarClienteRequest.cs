using MediatR;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Features.Clientes.CriarCliente;

public class CriarClienteRequest : IRequest<int>
{
    public string nomeCompleto { get; set; }
    public string cPF { get; set; }
    public string? telefone { get; set; }
    public string? whatsApp { get; set; }
    public string? email { get; set; }
    public DateTime? dataNascimento { get; set; }
    public Endereco? endereco { get; set; }
    public string? preferencias { get; set; }
    public string? observacoes { get; set; }
    public DateTime dataCadastro { get; set; }
    public string? rg { get; set; }
    public string? cnpj { get; set; }
    public string? FotoBase64 { get; set; }
}
