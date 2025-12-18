using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.UpdateCliente.Validators
{
    public class UpdateClienteHandler : IRequestHandler<UpdateClienteRequest, UpdateClienteResponse>
    {
        private readonly AppDbContext _context;

        public UpdateClienteHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateClienteResponse> Handle(UpdateClienteRequest request, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (cliente is null)
            {
                return new UpdateClienteResponse
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado."
                };
            }

            cliente.AtualizarDados(
                nomeCompleto: request.NomeCompleto,
                telefone: request.Telefone,
                whatsApp: request.WhatsApp,
                email: request.Email,
                cpf: request.Cpf,
                dataNascimento: request.DataNascimento,
                preferencias: request.Preferencias,
                observacoes: request.Observacoes,
                fotoBase64 : request.FotoUrl

            );

            if (cliente.Endereco is null)
            {
                cliente.Endereco = new Endereco(
                    rua: request.Rua,
                    numero: request.Numero,
                    bairro: request.Bairro,
                    cidade: request.Cidade,
                    cep: request.Cep,
                    complemento: request.Complemento
                );
            }
            else
            {
                cliente.Endereco.AtualizarEndereco(
                    rua: request.Rua,
                    numero: request.Numero,
                    bairro: request.Bairro,
                    cidade: request.Cidade,
                    cep: request.Cep,
                    complemento: request.Complemento
                );
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateClienteResponse
            {
                Sucesso = true,
                Mensagem = "Cliente atualizado com sucesso!",
                ClienteId = cliente.Id
            };
        }
    }
}
