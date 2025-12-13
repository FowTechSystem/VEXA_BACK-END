using CRMVEXA.Application.Commands.Clientes;
using CRMVEXA.Domain.Entities;
using MediatR;

public class CriarClienteCommandHandler : IRequestHandler<CriarClienteCommand, int>
{
    public async Task<int> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Cliente cliente = new();
        }
        catch
        {
        }
    }
}
