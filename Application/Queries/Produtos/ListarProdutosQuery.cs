using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Produtos;

public record ListarProdutosQuery() : IRequest<IEnumerable<ProdutoDto>>;
