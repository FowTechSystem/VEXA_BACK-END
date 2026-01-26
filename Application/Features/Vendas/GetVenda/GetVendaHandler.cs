using System;
using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaHandler : IRequestHandler<GetVendaRequest, GetVendaResponse>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetVendaHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetVendaResponse> Handle(GetVendaRequest request, CancellationToken cancellationToken)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            var vendaDto = _mapper.Map<VendaDto>(venda);

            return new GetVendaResponse { Venda = vendaDto };
        }
    }
}
