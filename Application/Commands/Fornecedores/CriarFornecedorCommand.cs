using MediatR;

namespace CRMVEXA.Application.Commands.Fornecedores
{
    public record CriarFornecedorCommand(
        string Nome,
        string? RazaoSocial,
        string? CNPJ,
        string? NomeRepresentante,
        string Telefone,
        string? WhatsApp,
        string? Email,
        string? Endereco,
        string? CategoriaProdutos,
        string? CondicaoPagamento,
        string? Observacoes
    ) : IRequest<int>;
}
