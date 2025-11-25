using MediatR;

namespace CRMVEXA.Application.Commands.Caixas
{
    public record AbrirCaixaCommand(
        int UsuarioId,
        decimal SaldoInicial
    ) : IRequest<int>;
}
