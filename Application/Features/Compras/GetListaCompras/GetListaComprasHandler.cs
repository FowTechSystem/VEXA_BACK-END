using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Compras.GetListaCompras;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasHandler : IRequestHandler<GetListaComprasRequest, GetListaComprasResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetListaComprasHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetListaComprasResponse> Handle(GetListaComprasRequest request, CancellationToken cancellationToken)
        {
            var compras = await _context.Compras
                .Include(c => c.Itens)
                .ToListAsync(cancellationToken);

            var comprasDto = _mapper.Map<List<CompraDto>>(compras);

            // Preencher dados relacionados
            foreach (var compraDto in comprasDto)
            {
                var fornecedor = await _context.Fornecedores.FindAsync(new object[] { compraDto.FornecedorId }, cancellationToken);
                if (fornecedor != null)
                {
                    compraDto.FornecedorNome = fornecedor.Nome;
                }

                var usuario = await _context.Usuarios.FindAsync(new object[] { compraDto.UsuarioId }, cancellationToken);
                if (usuario != null)
                {
                    compraDto.UsuarioNome = usuario.Nome;
                }

                foreach (var itemDto in compraDto.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(new object[] { itemDto.ProdutoId }, cancellationToken);
                    if (produto != null)
                    {
                        itemDto.ProdutoNome = produto.Nome;
                    }
                }
            }

            return new GetListaComprasResponse { Compras = comprasDto };
        }
    }
}
