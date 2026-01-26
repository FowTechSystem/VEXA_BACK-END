using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Vendas.GetListaVendas;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasHandler : IRequestHandler<GetListaVendasRequest, GetListaVendasResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetListaVendasHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetListaVendasResponse> Handle(GetListaVendasRequest request, CancellationToken cancellationToken)
        {
            var vendas = await _context.Vendas
                .Include(v => v.Itens)
                .ToListAsync(cancellationToken);

            var vendasDto = _mapper.Map<List<VendaDto>>(vendas);

            return new GetListaVendasResponse { Vendas = vendasDto };
        }
    }
}
