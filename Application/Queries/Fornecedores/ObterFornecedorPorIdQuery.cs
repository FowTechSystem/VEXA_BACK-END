using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Fornecedores;

public record ObterFornecedorPorIdQuery(int Id) : IRequest<FornecedorDto>;
