using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Caixa.GetResumo;

public class GetCaixaResumoHandler : IRequestHandler<GetCaixaResumoRequest, GetCaixaResumoResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetCaixaResumoHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetCaixaResumoResponse> Handle(GetCaixaResumoRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;

        var (inicio, fim) = ObterPeriodo(request.DataInicio, request.DataFim);

        var totalVendas = await _context.Vendas
            .Where(v => v.EmpresaId == empresaId
                && v.DataHora >= inicio
                && v.DataHora < fim
                && v.Status != "Cancelada")
            .SumAsync(v => v.ValorTotal, cancellationToken);

        var totalCondicionais = await _context.Condicionais
            .Where(c => c.EmpresaId == empresaId
                && c.Data >= inicio
                && c.Data < fim
                && c.Status == "pendente")
            .SumAsync(c => c.ValorTotal, cancellationToken);

        var totalSaidasCompras = await _context.Compras
            .Where(c => c.EmpresaId == empresaId
                && c.DataHora >= inicio
                && c.DataHora < fim)
            .SumAsync(c => c.ValorTotal, cancellationToken);

        var manuaisNoPeriodo = await _context.MovimentacoesCaixaManuais
            .Where(m => m.EmpresaId == empresaId
                && m.DataLancamento >= inicio
                && m.DataLancamento < fim)
            .ToListAsync(cancellationToken);

        var totalEntradasManuais = manuaisNoPeriodo
            .Where(m => m.TipoResumo == "entrada_manual")
            .Sum(m => m.Valor);
        var totalSaidasManuais = manuaisNoPeriodo
            .Where(m => m.TipoResumo == "saida_manual")
            .Sum(m => m.Valor);

        var totalEntradas = totalVendas + totalCondicionais + totalEntradasManuais;
        var totalSaidas = totalSaidasCompras + totalSaidasManuais;
        var saldoAtual = totalEntradas - totalSaidas;
        var totalEntradasMenosSaldo = totalEntradas - saldoAtual;

        var vendasEntidades = await _context.Vendas
            .Where(v => v.EmpresaId == empresaId
                && v.DataHora >= inicio
                && v.DataHora < fim
                && v.Status != "Cancelada")
            .ToListAsync(cancellationToken);

        var clienteIds = vendasEntidades.Where(v => v.ClienteId.HasValue).Select(v => v.ClienteId!.Value).Distinct().ToList();
        var clientes = clienteIds.Count > 0
            ? await _context.Clientes.Where(c => clienteIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.Cliente>();

        var vendas = vendasEntidades.Select(v =>
        {
            clientes.TryGetValue(v.ClienteId ?? 0, out var cli);
            var valor = Math.Abs(v.ValorTotal);
            return new MovimentacaoResumoItem
            {
                Id = v.Id,
                Tipo = "venda",
                Data = v.DataHora,
                Valor = valor,
                Descricao = "Venda #" + v.CodigoVenda,
                ClienteFornecedor = cli?.NomeCompleto,
                FormaPagamento = v.FormaPagamento,
                Status = v.Status,
                VendaId = v.Id,
                Natureza = "entrada"
            };
        }).ToList();

        var comprasEntidades = await _context.Compras
            .Where(c => c.EmpresaId == empresaId
                && c.DataHora >= inicio
                && c.DataHora < fim)
            .ToListAsync(cancellationToken);

        var fornecedorIds = comprasEntidades.Select(c => c.FornecedorId).Distinct().ToList();
        var fornecedores = await _context.Fornecedores
            .Where(f => fornecedorIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, cancellationToken);

        var compras = comprasEntidades.Select(c =>
        {
            fornecedores.TryGetValue(c.FornecedorId, out var forn);
            var valor = -Math.Abs(c.ValorTotal);
            return new MovimentacaoResumoItem
            {
                Id = c.Id,
                Tipo = "compra",
                Data = c.DataHora,
                Valor = valor,
                Descricao = string.IsNullOrWhiteSpace(c.Observacoes) ? "Compra " + c.CodigoCompra : c.Observacoes,
                ClienteFornecedor = forn?.Nome,
                FormaPagamento = null,
                Status = c.Status,
                CompraId = c.Id,
                Natureza = "saida"
            };
        }).ToList();

        var condicionaisEntidades = await _context.Condicionais
            .Where(c => c.EmpresaId == empresaId
                && c.Data >= inicio
                && c.Data < fim
                && c.Status == "pendente")
            .Include(c => c.Cliente)
            .ToListAsync(cancellationToken);

        var condicionais = condicionaisEntidades.Select(c => new MovimentacaoResumoItem
        {
            Id = c.Id,
            Tipo = "condicional",
            Data = c.Data,
            Valor = Math.Abs(c.ValorTotal),
            Descricao = "Condicional #" + c.Id,
            ClienteFornecedor = c.Cliente?.NomeCompleto,
            FormaPagamento = null,
            Status = c.Status,
            Natureza = "entrada"
        }).ToList();

        var manuaisItens = manuaisNoPeriodo.Select(m =>
        {
            var entrada = m.TipoResumo == "entrada_manual";
            var valorAbs = Math.Abs(m.Valor);
            return new MovimentacaoResumoItem
            {
                Id = m.Id,
                Tipo = m.TipoResumo,
                Data = m.DataLancamento,
                Valor = entrada ? valorAbs : -valorAbs,
                Descricao = m.Descricao,
                ClienteFornecedor = null,
                FormaPagamento = m.FormaPagamento,
                Status = null,
                Categoria = m.Categoria,
                Natureza = entrada ? "entrada" : "saida"
            };
        }).ToList();

        var movimentacoes = vendas
            .Concat(compras)
            .Concat(condicionais)
            .Concat(manuaisItens)
            .OrderByDescending(m => m.Data)
            .ToList();

        return new GetCaixaResumoResponse
        {
            TotalEntradas = totalEntradas,
            TotalSaidas = totalSaidas,
            SaldoAtual = saldoAtual,
            TotalEntradasMenosSaldo = totalEntradasMenosSaldo,
            Movimentacoes = movimentacoes
        };
    }

    private static (DateTime inicio, DateTime fim) ObterPeriodo(DateTime? dataInicio, DateTime? dataFim)
    {
        if (dataInicio.HasValue && dataFim.HasValue)
        {
            var inicio = dataInicio.Value;
            var fim = dataFim.Value.Date.AddDays(1);
            if (inicio.Kind == DateTimeKind.Unspecified) inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
            if (fim.Kind == DateTimeKind.Unspecified) fim = DateTime.SpecifyKind(fim, DateTimeKind.Utc);
            return (inicio, fim);
        }
        var now = DateTime.UtcNow;
        var primeiroDia = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = primeiroDia.AddMonths(1);
        return (primeiroDia, fimMes);
    }
}
