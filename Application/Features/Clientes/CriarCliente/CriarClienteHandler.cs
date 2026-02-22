using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

public class CriarClienteHandler : IRequestHandler<CriarClienteRequest, int>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public CriarClienteHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<int> Handle(CriarClienteRequest request, CancellationToken cancellationToken) 
    {
        try
        {
            var cliente = new Cliente
            {
                EmpresaId = _empresaContext.EmpresaId,
                NomeCompleto = request.nomeCompleto,
                CPF = request.cPF,
                Telefone = request.telefone,
                WhatsApp = request.whatsApp,
                Email = request.email,
                DataNascimento = request.dataNascimento?.ToUniversalTime(),
                Preferencias = request.preferencias,
                Observacoes = request.observacoes,
                DataCadastro = request.dataCadastro.ToUniversalTime(),
                RG = request.rg,
                CNPJ = request.cnpj,
                FotoBase64 = request.FotoBase64
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(cancellationToken);

            // Salvar endereço se informado
            if (request.endereco != null)
            {
                var rua = !string.IsNullOrWhiteSpace(request.endereco.Rua) ? request.endereco.Rua : (request.endereco.Endereco ?? "");
                var temEndereco = !string.IsNullOrWhiteSpace(rua) || !string.IsNullOrWhiteSpace(request.endereco.Cep) ||
                    !string.IsNullOrWhiteSpace(request.endereco.Numero) || !string.IsNullOrWhiteSpace(request.endereco.Bairro) ||
                    !string.IsNullOrWhiteSpace(request.endereco.Cidade);
                if (temEndereco)
                {
                    var endereco = new Endereco(
                        rua,
                        request.endereco.Numero ?? "",
                        request.endereco.Bairro ?? "",
                        request.endereco.Cidade ?? "",
                        request.endereco.Cep ?? "",
                        request.endereco.Complemento)
                    {
                        ClienteId = cliente.Id
                    };
                    _context.Enderecos.Add(endereco);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            return cliente.Id;
        }
        catch (Exception ex)
        {
            // TODO: Adicionar notificação Discord aqui depois
            // await _discordService.EnviarErro(ex);

            throw new Exception($"Erro ao criar cliente: {ex.Message}", ex);
        }
    }
}
