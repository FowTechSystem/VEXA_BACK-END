using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Clientes
{
    public class ObterClientePorIdQuery : IRequest<ClienteDto>
    {
        public int Id { get; set; }

        public ObterClientePorIdQuery(int id)
        {
            Id = id;
        }
    }
}
