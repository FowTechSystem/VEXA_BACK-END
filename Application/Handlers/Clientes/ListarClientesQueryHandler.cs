using MediatR;
using CRMVEXA.Application.Queries.Clientes;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CRMVEXA.Infrastructure.Data;

namespace CRMVEXA.Application.Handlers.Clientes
{
    public class ListarClientesQueryHandler : IRequestHandler<ListarClientesQuery, IEnumerable<ClienteDto>>
    {
        private readonly AppDbContext _context;

        public ListarClientesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _context.Clientes
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
                .ToListAsync(cancellationToken);

            return clientes;
        }
    }
}
