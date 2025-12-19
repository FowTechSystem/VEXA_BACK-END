using CRMVEXA.Application.Features.Fornecedor.UpdateFornecedor;
using MediatR;

namespace CRMVEXA.Application.Features.Fornecedor.UpdateClienteFornecedor;

public class UpdateFornecedorRequest : IRequest<UpdateFornecedorResponse>
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? RazaoSocial { get; set; }
    public string? CNPJ { get; set; }
    public string? NomeRepresentante { get; set; }
    public string Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? CategoriaProdutos { get; set; }
    public string? CondicaoPagamento { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; }
}
