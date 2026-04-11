using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

public class CriarClienteHandler : IRequestHandler<CriarClienteRequest, CriarClienteResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public CriarClienteHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<CriarClienteResponse> Handle(CriarClienteRequest request, CancellationToken cancellationToken) 
    {
        try
        {
            var fotoBytes = ClienteFotoPayloadHelper.TryDecodeToBytes(request.FotoBase64);

            var cliente = new Cliente
            {
                EmpresaId = _empresaContext.EmpresaId,
                NomeCompleto = request.nomeCompleto,
                CPF = request.cPF,
                Telefone = request.telefone ?? request.whatsApp,
                WhatsApp = request.whatsApp ?? request.telefone ?? "",
                Email = request.email,
                DataNascimento = request.dataNascimento?.ToUniversalTime(),
                Preferencias = request.preferencias,
                Observacoes = request.observacoes,
                DataCadastro = request.dataCadastro.ToUniversalTime(),
                RG = request.rg,
                CNPJ = request.cnpj,
                Foto = fotoBytes
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(cancellationToken);

            string? ruaVal = null;
            string? numeroVal = null;
            string? bairroVal = null;
            string? cidadeVal = null;
            string? cepVal = null;
            string? complementoVal = null;

            if (request.endereco != null)
            {
                ruaVal = !string.IsNullOrWhiteSpace(request.endereco.Rua) ? request.endereco.Rua : (request.endereco.Endereco ?? "");
                numeroVal = request.endereco.Numero;
                bairroVal = request.endereco.Bairro;
                cidadeVal = request.endereco.Cidade;
                cepVal = request.endereco.Cep;
                complementoVal = request.endereco.Complemento;
            }
            else if (!string.IsNullOrWhiteSpace(request.rua) || !string.IsNullOrWhiteSpace(request.cep) || !string.IsNullOrWhiteSpace(request.cidade))
            {
                ruaVal = request.rua;
                numeroVal = request.numero;
                bairroVal = request.bairro;
                cidadeVal = request.cidade;
                cepVal = request.cep;
                complementoVal = request.complemento;
            }

            var temEndereco = !string.IsNullOrWhiteSpace(ruaVal) || !string.IsNullOrWhiteSpace(cepVal) ||
                !string.IsNullOrWhiteSpace(numeroVal) || !string.IsNullOrWhiteSpace(bairroVal) || !string.IsNullOrWhiteSpace(cidadeVal);
            if (temEndereco)
            {
                var endereco = new Endereco(
                    ruaVal ?? "",
                    numeroVal ?? "",
                    bairroVal ?? "",
                    cidadeVal ?? "",
                    cepVal ?? "",
                    complementoVal)
                {
                    ClienteId = cliente.Id
                };
                _context.Enderecos.Add(endereco);
                await _context.SaveChangesAsync(cancellationToken);

                return new CriarClienteResponse
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
                    Enderecos = new List<EnderecoClienteCriarResponse>
                    {
                        new() { Rua = endereco.Rua, Numero = endereco.Numero, Complemento = endereco.Complemento, Bairro = endereco.Bairro, Cidade = endereco.Cidade, Cep = endereco.Cep }
                    }
                };
            }

            return new CriarClienteResponse
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
                Enderecos = new List<EnderecoClienteCriarResponse>()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar cliente: {ex.Message}", ex);
        }
    }
}
