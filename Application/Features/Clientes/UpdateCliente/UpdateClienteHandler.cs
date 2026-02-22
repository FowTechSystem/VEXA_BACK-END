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

            // Usar endereço: Endereco (objeto) > Enderecos[0] > campos flat
            var e = request.Endereco ?? request.Enderecos?.FirstOrDefault();
            var rua = e != null
                ? (!string.IsNullOrWhiteSpace(e.Rua) ? e.Rua : (e.Endereco ?? ""))
                : (request.Rua ?? "");
            var numero = e?.Numero ?? request.Numero ?? "";
            var bairro = e?.Bairro ?? request.Bairro ?? "";
            var cidade = e?.Cidade ?? request.Cidade ?? "";
            var cep = e?.Cep ?? request.Cep ?? "";
            var complemento = e?.Complemento ?? request.Complemento;

            var temEndereco = !string.IsNullOrWhiteSpace(rua) || !string.IsNullOrWhiteSpace(cep) ||
                !string.IsNullOrWhiteSpace(numero) || !string.IsNullOrWhiteSpace(bairro) || !string.IsNullOrWhiteSpace(cidade);

            if (temEndereco)
            {
                var enderecoExistente = await _context.Set<Endereco>()
                    .FirstOrDefaultAsync(e =>
                        e.Rua == rua &&
                        e.Numero == numero &&
                        e.Bairro == bairro &&
                        e.Cidade == cidade &&
                        e.Cep == cep &&
                        (e.Complemento ?? "") == (complemento ?? ""),
                        cancellationToken);

                if (enderecoExistente != null)
                {
                    if (!cliente.Enderecos.Any(e => e.Id == enderecoExistente.Id))
                    {
                        cliente.AdicionarEndereco(enderecoExistente);
                    }
                    else
                    {
                        enderecoExistente.AtualizarEndereco(rua, numero, bairro, cidade, cep, complemento);
                    }
                }
                else
                {
                    var novoEndereco = new Endereco(rua, numero, bairro, cidade, cep, complemento);
                    cliente.AdicionarEndereco(novoEndereco);
                }
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
