using CRMVEXA.Application.Features.Vendas.GetListaVendas;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasHandler : IRequestHandler<GetListaVendasRequest, GetListaVendasResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetListaVendasHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetListaVendasResponse> Handle(GetListaVendasRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var query = _context.Vendas.Where(v => v.EmpresaId == empresaId).Include(v => v.Itens).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                var valor = request.Valor.ToLower();
                query = query.Where(v => v.CodigoVenda.ToLower().Contains(valor));
            }

            if (request.Mes.HasValue && request.Ano.HasValue)
            {
                var m = Math.Clamp(request.Mes.Value, 1, 12);
                var inicio = new DateTime(request.Ano.Value, m, 1, 0, 0, 0, DateTimeKind.Utc);
                var fim = inicio.AddMonths(1);
                query = query.Where(v => v.DataHora >= inicio && v.DataHora < fim);
            }
            else
            {
                if (request.DataInicio.HasValue)
                {
                    var inicio = request.DataInicio.Value;
                    if (inicio.Kind == DateTimeKind.Unspecified)
                        inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
                    query = query.Where(v => v.DataHora >= inicio);
                }
                if (request.DataFim.HasValue)
                {
                    var fim = request.DataFim.Value.Date.AddDays(1);
                    if (fim.Kind == DateTimeKind.Unspecified)
                        fim = DateTime.SpecifyKind(fim, DateTimeKind.Utc);
                    query = query.Where(v => v.DataHora < fim);
                }
            }

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 500);

            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var vendas = await query
                .OrderByDescending(v => v.DataHora)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);

            var vendasResponse = new List<VendaResponse>();
            foreach (var v in vendas)
            {
                var cliente = v.ClienteId.HasValue ? await _context.Clientes.FindAsync(new object[] { v.ClienteId.Value }, cancellationToken) : null;
                var usuario = await _context.Usuarios.FindAsync(new object[] { v.UsuarioId }, cancellationToken);
                var vendedorRef = usuario != null ? new VendedorRefResponse { Id = usuario.Id, Nome = usuario.Nome ?? "" } : null;

                var itensResp = new List<ItemVendaResponse>();
                foreach (var i in v.Itens)
                {
                    var produto = await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == i.ProdutoId, cancellationToken);
                    itensResp.Add(new ItemVendaResponse
                    {
                        Id = i.Id,
                        ProdutoId = i.ProdutoId,
                        Produto = produto != null ? new ProdutoRefResponse
                        {
                            Id = produto.Id,
                            Nome = produto.Nome,
                            Sku = produto.SKU ?? "",
                            PrecoVenda = produto.PrecoVenda,
                            PrecoCusto = produto.PrecoCusto,
                            Categoria = produto.Categoria != null ? new CategoriaRefResponse { Id = produto.Categoria.Id, Nome = produto.Categoria.Nome } : null
                        } : null,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        Subtotal = i.Subtotal
                    });
                }
                vendasResponse.Add(new VendaResponse
                {
                    Id = v.Id,
                    CodigoVenda = v.CodigoVenda,
                    ClienteId = v.ClienteId,
                    Cliente = cliente != null ? new ClienteRefResponse { Id = cliente.Id, Nome = cliente.NomeCompleto } : null,
                    NomeCliente = cliente?.NomeCompleto,
                    Desconto = v.Desconto ?? 0,
                    FormaPagamento = v.FormaPagamento,
                    UsuarioId = v.UsuarioId,
                    Vendedor = vendedorRef,
                    Usuario = vendedorRef,
                    DataHora = v.DataHora,
                    ValorTotal = v.ValorTotal,
                    Status = v.Status,
                    Observacoes = null,
                    Itens = itensResp
                });
            }

            return new GetListaVendasResponse
            {
                Vendas = vendasResponse,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = tamanho
            };
        }
    }
}
