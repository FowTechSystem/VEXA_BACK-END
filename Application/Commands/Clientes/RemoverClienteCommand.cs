using MediatR;

namespace CRMVEXA.Application.Commands.Clientes
{
    public class DeletarClienteCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeletarClienteCommand(int id)
        {
            Id = id;
        }
    }
}
