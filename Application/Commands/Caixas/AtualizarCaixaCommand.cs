using MediatR;

namespace CRMVEXA.Application.Commands.Caixas
{
    public record AtualizarCaixaCommand(
        int Id,
        DateTime DataAbertura,
        DateTime? DataFechamento,
        decimal SaldoInicial,
        decimal SaldoFinal,
        int UsuarioId,
        string? Observacoes,
        bool Aberto
    ) : IRequest<bool>;
}
