using MediatR;

namespace CRMVEXA.Application.Commands.Clientes
{
    public record CriarClienteCommand(
        string NomeCompleto,
        string Telefone,
        string WhatsApp,
        string? Email,
        string? CPF,
        DateTime? DataNascimento,
        string Rua,
        string Numero,
        string Bairro,
        string Cidade,
        string CEP,
        string? Preferencias,
        string? Observacoes
    ) : IRequest<int>;
}
