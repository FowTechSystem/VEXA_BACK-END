using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.GetListaUsuarios
{
    public class GetListaUsuariosRequest : IRequest<GetListaUsuariosResponse>
    {
        public string? Filtro { get; set; }
        public string? Valor { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 10;
    }
}
