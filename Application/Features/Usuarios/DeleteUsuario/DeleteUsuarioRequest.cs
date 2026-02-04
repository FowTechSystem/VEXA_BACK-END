using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioRequest : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
