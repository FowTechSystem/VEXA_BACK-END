using MediatR;

namespace CRMVEXA.Application.Commands.Caixas
{
    public record FecharCaixaCommand(
        int CaixaId,
        decimal SaldoFinal,
        string? Observacoes
    ) : IRequest<bool>;
}
