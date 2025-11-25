using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Fornecedores;

public record ListarFornecedoresQuery() : IRequest<IEnumerable<FornecedorDto>>;
