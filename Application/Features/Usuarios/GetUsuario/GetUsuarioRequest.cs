using MediatR;
using CRMVEXA.Application.Features.Usuarios.GetListaUsuarios;

namespace CRMVEXA.Application.Features.Usuarios.GetUsuario
{
    public class GetUsuarioRequest : IRequest<UsuarioResponse?>
    {
        public int Id { get; set; }
    }
}
