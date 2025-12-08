using MediatR;
using CRMVEXA.Application.Queries.Clientes;
using CRMVEXA.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using CRMVEXA.Infrastructure.Data;

namespace CRMVEXA.Application.Handlers.Clientes
{
    public class ObterClientePorIdQueryHandler : IRequestHandler<ObterClientePorIdQuery, ClienteDto>
    {
        private readonly AppDbContext _context;

        public ObterClientePorIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClienteDto> Handle(ObterClientePorIdQuery request, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes
                .Where(c => c.Id == request.Id)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    NomeCompleto = c.NomeCompleto,
                    Email = c.Email,
                    Telefone = c.Telefone,
                    CPF = c.CPF,
                    Endereco = c.Endereco != null ? new EnderecoDto
                    {
                        Rua = c.Endereco.Rua,
                        Numero = c.Endereco.Numero,
                        Bairro = c.Endereco.Bairro,
                        Cidade = c.Endereco.Cidade,
                        CEP = c.Endereco.CEP
                    } : new EnderecoDto(),
                })
                .FirstOrDefaultAsync(cancellationToken);

            return cliente;
        }
    }
}
