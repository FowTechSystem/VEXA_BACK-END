using CRMVEXA.Application.Features.Vendas;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaHandler : IRequestHandler<GetVendaRequest, GetVendaResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetVendaHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetVendaResponse> Handle(GetVendaRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var venda = await _context.Vendas
                .Where(v => v.EmpresaId == empresaId)
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            var usuario = await _context.Usuarios.FindAsync(new object[] { venda.UsuarioId }, cancellationToken);
            var vendedorRef = usuario != null ? new VendedorRefResponse { Id = usuario.Id, Nome = usuario.Nome ?? "" } : null;

            var itensResp = await VendaItemMapper.MapItensAsync(_context, empresaId, venda.Itens, cancellationToken);
            return new GetVendaResponse
            {
                Id = venda.Id,
                CodigoVenda = venda.CodigoVenda,
                ClienteId = venda.ClienteId,
                Desconto = venda.Desconto ?? 0,
                FormaPagamento = venda.FormaPagamento,
                UsuarioId = venda.UsuarioId,
                Vendedor = vendedorRef,
                Usuario = vendedorRef,
                DataHora = venda.DataHora,
                ValorTotal = venda.ValorTotal,
                Status = venda.Status,
                Itens = itensResp
            };
        }
    }
}
