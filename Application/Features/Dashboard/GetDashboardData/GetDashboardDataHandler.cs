using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Dashboard.GetDashboardData;

public class GetDashboardDataHandler : IRequestHandler<GetDashboardDataRequest, GetDashboardDataResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetDashboardDataHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetDashboardDataResponse> Handle(GetDashboardDataRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;
        var mes = Math.Clamp(request.Mes, 1, 12);
        var ano = request.Ano;

        var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1);

        var vendasDoMes = await _context.Vendas
            .Where(v => v.EmpresaId == empresaId
                && v.DataHora >= inicioMes
                && v.DataHora < fimMes
                && v.Status != "Cancelada")
            .Include(v => v.Itens)
            .ToListAsync(cancellationToken);

        var vendaIds = vendasDoMes.Select(v => v.Id).ToList();
        var itensVenda = vendasDoMes.SelectMany(v => v.Itens).Where(i => i.ProdutoId.HasValue).ToList();
        var produtoIds = itensVenda.Select(i => i.ProdutoId!.Value).Distinct().ToList();

        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id))
            .Include(p => p.Categoria)
            .ToListAsync(cancellationToken);

        var produtoDict = produtos.ToDictionary(p => p.Id);

        decimal faturamentoMesLucro = 0;
        decimal faturamentoMesBruto = vendasDoMes.Sum(v => v.ValorTotal);

        foreach (var item in itensVenda)
        {
            if (!item.ProdutoId.HasValue || !produtoDict.TryGetValue(item.ProdutoId.Value, out var produto)) continue;
            var lucroUnitario = item.PrecoUnitario - produto.PrecoCusto;
            faturamentoMesLucro += lucroUnitario * item.Quantidade;
        }

        var totalVendas = vendasDoMes.Count;

        var clientesDoMes = vendasDoMes
            .Where(v => v.ClienteId.HasValue)
            .Select(v => v.ClienteId!.Value)
            .Distinct()
            .ToList();

        var clientesQueCompraremAntes = await _context.Vendas
            .Where(v => v.EmpresaId == empresaId
                && v.ClienteId.HasValue
                && v.DataHora < inicioMes
                && v.Status != "Cancelada")
            .Select(v => v.ClienteId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var novosClientes = clientesDoMes.Except(clientesQueCompraremAntes).Count();

        var ticketMedio = totalVendas > 0 ? faturamentoMesBruto / totalVendas : 0;

        var faturamentoMensal = new List<FaturamentoMensalItem>();
        for (var m = 1; m <= 12; m++)
        {
            var inicio = new DateTime(ano, m, 1, 0, 0, 0, DateTimeKind.Utc);
            var fim = inicio.AddMonths(1);
            var valor = await _context.Vendas
                .Where(v => v.EmpresaId == empresaId
                    && v.DataHora >= inicio
                    && v.DataHora < fim
                    && v.Status != "Cancelada")
                .SumAsync(v => v.ValorTotal, cancellationToken);
            faturamentoMensal.Add(new FaturamentoMensalItem { Mes = m, Ano = ano, ValorBruto = valor });
        }

        var vendasPorCategoria = itensVenda
            .Where(i => i.ProdutoId.HasValue)
            .Select(i =>
            {
                produtoDict.TryGetValue(i.ProdutoId!.Value, out var p);
                return new { CatId = p?.CategoriaId ?? 0, CatNome = p?.Categoria?.Nome ?? "Sem categoria", Qtd = i.Quantidade, Valor = i.Subtotal };
            })
            .GroupBy(x => new { x.CatId, x.CatNome })
            .Select(g => new VendasPorCategoriaItem
            {
                CategoriaId = g.Key.CatId,
                CategoriaNome = g.Key.CatNome,
                QuantidadeVendida = g.Sum(x => x.Qtd),
                ValorVendido = g.Sum(x => x.Valor)
            })
            .OrderByDescending(x => x.ValorVendido)
            .ToList();

        var produtosMaisVendidos = itensVenda
            .Where(i => i.ProdutoId.HasValue)
            .GroupBy(i => i.ProdutoId!.Value)
            .Select(g =>
            {
                produtoDict.TryGetValue(g.Key, out var p);
                return new ProdutoMaisVendidoItem
                {
                    ProdutoId = g.Key,
                    ProdutoNome = p?.Nome ?? "?",
                    Sku = p?.SKU ?? "",
                    QuantidadeVendida = g.Sum(x => x.Quantidade),
                    ValorVendido = g.Sum(x => x.Subtotal)
                };
            })
            .OrderByDescending(x => x.QuantidadeVendida)
            .Take(4)
            .ToList();

        var estoqueCritico = await _context.Estoques
            .Where(e => e.EmpresaId == empresaId)
            .Include(e => e.Produto)
            .ToListAsync(cancellationToken);

        var estoqueCriticoLista = estoqueCritico
            .Where(e => e.EstoqueMinimo > 0 && e.Quantidade < e.EstoqueMinimo)
            .Select(e => new EstoqueCriticoItem
            {
                ProdutoId = e.ProdutoId,
                ProdutoNome = e.Produto?.Nome ?? "?",
                Sku = e.Produto?.SKU ?? "",
                QuantidadeAtual = e.Quantidade,
                EstoqueMinimo = e.EstoqueMinimo,
                Status = "CRITICO"
            })
            .OrderBy(x => x.QuantidadeAtual)
            .ToList();

        var vendedoresDoMes = vendasDoMes
            .GroupBy(v => v.UsuarioId)
            .Select(g => new { UsuarioId = g.Key, ValorTotal = g.Sum(v => v.ValorTotal), QtdVendas = g.Count() })
            .OrderByDescending(x => x.ValorTotal)
            .Take(3)
            .ToList();

        var usuarioIds = vendedoresDoMes.Select(x => x.UsuarioId).Distinct().ToList();
        var usuarios = await _context.Usuarios
            .Where(u => usuarioIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var topVendedores = vendedoresDoMes
            .Select(x =>
            {
                usuarios.TryGetValue(x.UsuarioId, out var u);
                return new TopVendedorItem
                {
                    UsuarioId = x.UsuarioId,
                    Nome = u?.Nome ?? "?",
                    QuantidadeVendas = x.QtdVendas,
                    ValorTotalVendido = x.ValorTotal
                };
            })
            .ToList();

        var totalFormas = vendasDoMes.Sum(v => v.ValorTotal);
        var formasPagamento = vendasDoMes
            .GroupBy(v => NormalizarFormaPagamento(v.FormaPagamento))
            .Select(g => new FormaPagamentoItem
            {
                FormaPagamento = g.Key,
                Valor = g.Sum(v => v.ValorTotal),
                Percentual = totalFormas > 0 ? (g.Sum(v => v.ValorTotal) / totalFormas) * 100 : 0
            })
            .OrderByDescending(x => x.Valor)
            .ToList();

        return new GetDashboardDataResponse
        {
            Mes = mes,
            Ano = ano,
            FaturamentoMesLucro = faturamentoMesLucro,
            TotalVendas = totalVendas,
            NovosClientes = novosClientes,
            FaturamentoMesBruto = faturamentoMesBruto,
            TicketMedio = ticketMedio,
            FaturamentoMensal = faturamentoMensal,
            VendasPorCategoria = vendasPorCategoria,
            ProdutosMaisVendidos = produtosMaisVendidos,
            EstoqueCritico = estoqueCriticoLista,
            TopVendedores = topVendedores,
            FormasPagamento = formasPagamento
        };
    }

    private static string NormalizarFormaPagamento(string forma)
    {
        if (string.IsNullOrWhiteSpace(forma)) return "Outros";
        var f = forma.Trim();
        if (f.Contains("Credito", StringComparison.OrdinalIgnoreCase) || f.Equals("CartaoCredito", StringComparison.OrdinalIgnoreCase)) return "Crédito";
        if (f.Contains("Debito", StringComparison.OrdinalIgnoreCase) || f.Equals("CartaoDebito", StringComparison.OrdinalIgnoreCase)) return "Débito";
        if (f.Equals("Pix", StringComparison.OrdinalIgnoreCase)) return "Pix";
        if (f.Equals("Dinheiro", StringComparison.OrdinalIgnoreCase)) return "Dinheiro";
        if (f.Equals("Crediario", StringComparison.OrdinalIgnoreCase)) return "Crediário";
        return forma;
    }
}
