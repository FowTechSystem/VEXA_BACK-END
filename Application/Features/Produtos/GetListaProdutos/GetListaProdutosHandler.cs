using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Produtos.GetListaProdutos;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
    public class GetListaProdutosHandler : IRequestHandler<GetListaProdutosRequest, GetListaProdutosResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetListaProdutosHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetListaProdutosResponse> Handle(GetListaProdutosRequest request, CancellationToken cancellationToken)
        {
            var produtos = await _context.Produtos.ToListAsync(cancellationToken);
            var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

            return new GetListaProdutosResponse { Produtos = produtosDto };
        }
    }
}
