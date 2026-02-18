using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.UpdateCliente.Validators
{
    public class UpdateClienteHandler : IRequestHandler<UpdateClienteRequest, UpdateClienteResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public UpdateClienteHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<UpdateClienteResponse> Handle(UpdateClienteRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var cliente = await _context.Clientes
                .Where(c => c.EmpresaId == empresaId)
                .Include(c => c.Enderecos)
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
                rg: request.Rg,
                cnpj: request.Cnpj,
                dataNascimento: request.DataNascimento,
                preferencias: request.Preferencias,
                observacoes: request.Observacoes,
                fotoBase64: request.FotoUrl
            );

            var enderecoExistente = await _context.Set<Endereco>()
                .FirstOrDefaultAsync(e =>
                    e.Rua == request.Rua &&
                    e.Numero == request.Numero &&
                    e.Bairro == request.Bairro &&
                    e.Cidade == request.Cidade &&
                    e.Cep == request.Cep &&
                    e.Complemento == request.Complemento,
                    cancellationToken);

            if (enderecoExistente != null)
            {
                if (!cliente.Enderecos.Any(e => e.Id == enderecoExistente.Id))
                {
                    cliente.AdicionarEndereco(enderecoExistente);
                }
                else
                {
                    enderecoExistente.AtualizarEndereco(
                        request.Rua,
                        request.Numero,
                        request.Bairro,
                        request.Cidade,
                        request.Cep,
                        request.Complemento
                    );
                }
            }
            else
            {
                var novoEndereco = new Endereco(
                    rua: request.Rua,
                    numero: request.Numero,
                    bairro: request.Bairro,
                    cidade: request.Cidade,
                    cep: request.Cep,
                    complemento: request.Complemento
                );

                cliente.AdicionarEndereco(novoEndereco);
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
