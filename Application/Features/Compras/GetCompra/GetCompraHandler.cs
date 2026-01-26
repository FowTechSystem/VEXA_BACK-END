using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.GetCompra
{
    public class GetCompraHandler : IRequestHandler<GetCompraRequest, GetCompraResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetCompraHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetCompraResponse> Handle(GetCompraRequest request, CancellationToken cancellationToken)
        {
            var compra = await _context.Compras
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
            {
                throw new Exception($"Compra com ID {request.Id} não encontrada");
            }

            var compraDto = _mapper.Map<CompraDto>(compra);

            // Preencher dados relacionados
            var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compra.FornecedorId }, cancellationToken);
            if (fornecedor != null)
            {
                compraDto.FornecedorNome = fornecedor.Nome;
            }

            var usuario = await _context.Usuarios.FindAsync(new object[] { compra.UsuarioId }, cancellationToken);
            if (usuario != null)
            {
                compraDto.UsuarioNome = usuario.Nome;
            }

            // Preencher nomes dos produtos nos itens
            foreach (var itemDto in compraDto.Itens)
            {
                var produto = await _context.Produtos.FindAsync(new object[] { itemDto.ProdutoId }, cancellationToken);
                if (produto != null)
                {
                    itemDto.ProdutoNome = produto.Nome;
                }
            }

            return new GetCompraResponse { Compra = compraDto };
        }
    }
}
