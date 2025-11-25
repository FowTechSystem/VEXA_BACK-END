using MediatR;

namespace CRMVEXA.Application.Commands.Usuarios
{
    public record ExcluirUsuarioCommand(
        int Id // Id do usuário para excluir
    ) : IRequest<bool>;
}
