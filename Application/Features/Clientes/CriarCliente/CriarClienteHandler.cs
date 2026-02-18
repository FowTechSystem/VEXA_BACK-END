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
