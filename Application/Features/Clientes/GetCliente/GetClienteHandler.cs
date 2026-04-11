using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.GetCliente;

public class GetClienteHandler : IRequestHandler<GetClienteRequest, GetClienteResponse?>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetClienteHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetClienteResponse?> Handle(GetClienteRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var cliente = await _context.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Id == request.Id)
            .Include(c => c.Enderecos)
            .FirstOrDefaultAsync(cancellationToken);

        if (cliente == null)
            return null;

        return new GetClienteResponse
        {
            Id = cliente.Id,
            NomeCompleto = cliente.NomeCompleto,
            Email = cliente.Email,
            CPF = cliente.CPF,
            RG = cliente.RG,
            CNPJ = cliente.CNPJ,
            DataNascimento = cliente.DataNascimento,
            Telefone = cliente.Telefone,
            WhatsApp = cliente.WhatsApp,
            Preferencias = cliente.Preferencias,
            Observacoes = cliente.Observacoes,
            FotoUrl = ClienteFotoUrlHelper.ToApiFotoUrl(cliente.Foto),
            DataCadastro = cliente.DataCadastro,
            Enderecos = cliente.Enderecos?.Select(e => new EnderecoClienteResponse
            {
                Rua = e.Rua,
                Numero = e.Numero,
                Complemento = e.Complemento,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Cep = e.Cep
            }).ToList() ?? new List<EnderecoClienteResponse>()
        };
    }
}
