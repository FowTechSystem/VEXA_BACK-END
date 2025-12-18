using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

public class CriarClienteHandler : IRequestHandler<CriarClienteRequest, int>
{
    private readonly AppDbContext _context;

    public CriarClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CriarClienteRequest request, CancellationToken cancellationToken) 
    {
        try
        {
            var cliente = new Cliente
            {
                NomeCompleto = request.nomeCompleto,
                CPF = request.cPF,
                Telefone = request.telefone,
                WhatsApp = request.whatsApp,
                Email = request.email,
                DataNascimento = request.dataNascimento?.ToUniversalTime(),
                Endereco = request.endereco,
                Preferencias = request.preferencias,
                Observacoes = request.observacoes,
                DataCadastro = request.dataCadastro.ToUniversalTime(),
                RG = request.rg,
                CNPJ = request.cnpj,
                FotoBase64 = request.FotoBase64
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(cancellationToken);

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
