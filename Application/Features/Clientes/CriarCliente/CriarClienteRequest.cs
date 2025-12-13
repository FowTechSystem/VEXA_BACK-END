using CRMVEXA.Domain.Entities;
using MediatR;

namespace CRMVEXA.Application.Features.Clientes.CriarCliente;

public sealed record CriarClienteRequest(string nomeCompleto, string cPF, string? telefone, string? whatsApp, string? email,
        DateTime? dataNascimento, Endereco? endereco, string? preferencias, string? observacoes, DateTime dataCadastro) : IRequest<int>;

