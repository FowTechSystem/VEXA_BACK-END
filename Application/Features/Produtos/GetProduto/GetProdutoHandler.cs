using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Produtos.GetProduto;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoHandler : IRequestHandler<GetProdutoRequest, GetProdutoResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetProdutoHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetProdutoResponse> Handle(GetProdutoRequest request, CancellationToken cancellationToken)
        {
            var produto = await _context.Produtos.FindAsync(new object[] { request.Id }, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            var produtoDto = _mapper.Map<ProdutoDto>(produto);

            return new GetProdutoResponse { Produto = produtoDto };
        }
    }
}
