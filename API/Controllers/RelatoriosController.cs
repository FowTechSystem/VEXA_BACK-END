using System.Globalization;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public RelatoriosController(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    [HttpGet("vendas-periodo")]
    public async Task<ActionResult<VendasPeriodoResponse>> VendasPeriodo(
        [FromQuery] string dataInicio,
        [FromQuery] string dataFim,
        CancellationToken cancellationToken)
    {
        if (!TryParsePeriodo(dataInicio, dataFim, out var periodo, out var erro))
            return BadRequest(new { sucesso = false, mensagem = erro, message = erro });

        var empresaId = _empresaContext.EmpresaId;

        var vendasBase = await _context.Vendas
            .AsNoTracking()
            .Where(v =>
                v.EmpresaId == empresaId &&
                v.Status != "Cancelada" &&
                v.DataHora >= periodo.InicioUtc &&
                v.DataHora < periodo.FimExclusivoUtc)
            .Select(v => new
            {
                v.Id,
                v.DataHora,
                v.ClienteId,
                v.ValorTotal
            })
            .OrderByDescending(v => v.DataHora)
            .ToListAsync(cancellationToken);

        var vendaIds = vendasBase.Select(v => v.Id).ToList();
        var clienteIds = vendasBase.Where(v => v.ClienteId.HasValue).Select(v => v.ClienteId!.Value).Distinct().ToList();

        var clientesPorId = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.EmpresaId == empresaId && clienteIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.NomeCompleto, cancellationToken);

        var itensVenda = await _context.ItensVenda
            .AsNoTracking()
            .Where(i => vendaIds.Contains(i.VendaId))
            .Select(i => new
            {
                i.VendaId,
                i.ProdutoId,
                i.Quantidade,
                Subtotal = i.Quantidade * i.PrecoUnitario
            })
            .ToListAsync(cancellationToken);

        var produtoIds = itensVenda.Select(i => i.ProdutoId).Distinct().ToList();
        var produtosPorId = await _context.Produtos
            .AsNoTracking()
            .Where(p => p.EmpresaId == empresaId && produtoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Nome, cancellationToken);

        var itensPorVenda = itensVenda
            .GroupBy(i => i.VendaId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(i => new VendasPeriodoItemResponse
                {
                    Produto = produtosPorId.GetValueOrDefault(i.ProdutoId, "Produto removido"),
                    Quantidade = i.Quantidade,
                    Subtotal = i.Subtotal
                }).ToList());

        var vendas = vendasBase.Select(v => new VendasPeriodoVendaResponse
        {
            Id = v.Id,
            Data = v.DataHora.ToString("yyyy-MM-dd"),
            Cliente = v.ClienteId.HasValue ? clientesPorId.GetValueOrDefault(v.ClienteId.Value, "Cliente removido") : "Sem cliente",
            Valor = v.ValorTotal,
            Itens = itensPorVenda.GetValueOrDefault(v.Id, new List<VendasPeriodoItemResponse>())
        }).ToList();

        return Ok(new VendasPeriodoResponse
        {
            DataInicio = periodo.DataInicio,
            DataFim = periodo.DataFim,
            TotalVendas = vendas.Sum(v => v.Valor),
            QuantidadeVendas = vendas.Count,
            Vendas = vendas
        });
    }

    [HttpGet("produtos-estoque")]
    public async Task<ActionResult<ProdutosEstoqueResponse>> ProdutosEstoque(CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;

        var produtos = await _context.Produtos
            .AsNoTracking()
            .Where(p => p.EmpresaId == empresaId)
            .Select(p => new
            {
                Produto = p.Nome,
                Sku = p.SKU ?? string.Empty,
                Atual = p.Estoque != null ? p.Estoque.Quantidade : 0,
                Minimo = p.Estoque != null ? p.Estoque.EstoqueMinimo : 0
            })
            .OrderBy(p => p.Produto)
            .ToListAsync(cancellationToken);

        var itens = produtos.Select(p =>
        {
            var critico = p.Atual <= p.Minimo;
            return new ProdutosEstoqueItemResponse
            {
                Produto = p.Produto,
                Sku = p.Sku,
                Atual = p.Atual,
                Minimo = p.Minimo,
                Status = critico ? "Critico" : "Normal"
            };
        }).ToList();

        return Ok(new ProdutosEstoqueResponse
        {
            TotalProdutos = itens.Count,
            Criticos = itens.Count(p => p.Status == "Critico"),
            Normais = itens.Count(p => p.Status == "Normal"),
            Produtos = itens
        });
    }

    [HttpGet("vendas-por-cliente")]
    public async Task<ActionResult<VendasPorClienteResponse>> VendasPorCliente(
        [FromQuery] string dataInicio,
        [FromQuery] string dataFim,
        CancellationToken cancellationToken)
    {
        if (!TryParsePeriodo(dataInicio, dataFim, out var periodo, out var erro))
            return BadRequest(new { sucesso = false, mensagem = erro, message = erro });

        var empresaId = _empresaContext.EmpresaId;

        var agregados = await (
            from v in _context.Vendas.AsNoTracking()
            join c in _context.Clientes.AsNoTracking() on v.ClienteId equals c.Id into clientes
            from c in clientes.DefaultIfEmpty()
            where v.EmpresaId == empresaId
                  && v.Status != "Cancelada"
                  && v.DataHora >= periodo.InicioUtc
                  && v.DataHora < periodo.FimExclusivoUtc
            group v by c != null ? c.NomeCompleto : "Sem cliente" into g
            orderby g.Sum(x => x.ValorTotal) descending
            select new VendasPorClienteItemResponse
            {
                Cliente = g.Key,
                Pedidos = g.Count(),
                Valor = g.Sum(x => x.ValorTotal),
                TicketMedio = g.Count() == 0 ? 0 : Math.Round(g.Sum(x => x.ValorTotal) / g.Count(), 2)
            })
            .ToListAsync(cancellationToken);

        return Ok(new VendasPorClienteResponse
        {
            DataInicio = periodo.DataInicio,
            DataFim = periodo.DataFim,
            Clientes = agregados
        });
    }

    [HttpGet("produtos-mais-vendidos")]
    public async Task<ActionResult<ProdutosMaisVendidosResponse>> ProdutosMaisVendidos(
        [FromQuery] string dataInicio,
        [FromQuery] string dataFim,
        CancellationToken cancellationToken)
    {
        if (!TryParsePeriodo(dataInicio, dataFim, out var periodo, out var erro))
            return BadRequest(new { sucesso = false, mensagem = erro, message = erro });

        var empresaId = _empresaContext.EmpresaId;

        var produtos = await (
            from v in _context.Vendas.AsNoTracking()
            join i in _context.ItensVenda.AsNoTracking() on v.Id equals i.VendaId
            join p in _context.Produtos.AsNoTracking() on i.ProdutoId equals p.Id
            where v.EmpresaId == empresaId
                  && p.EmpresaId == empresaId
                  && v.Status != "Cancelada"
                  && v.DataHora >= periodo.InicioUtc
                  && v.DataHora < periodo.FimExclusivoUtc
            group new { i, p } by p.Nome into g
            orderby g.Sum(x => x.i.Quantidade) descending
            select new ProdutosMaisVendidosItemResponse
            {
                Produto = g.Key,
                Quantidade = g.Sum(x => x.i.Quantidade),
                Valor = g.Sum(x => x.i.Quantidade * x.i.PrecoUnitario)
            })
            .ToListAsync(cancellationToken);

        return Ok(new ProdutosMaisVendidosResponse
        {
            DataInicio = periodo.DataInicio,
            DataFim = periodo.DataFim,
            Produtos = produtos
        });
    }

    private static bool TryParsePeriodo(string? dataInicio, string? dataFim, out PeriodoRelatorio periodo, out string erro)
    {
        periodo = default;
        erro = string.Empty;

        if (string.IsNullOrWhiteSpace(dataInicio) || string.IsNullOrWhiteSpace(dataFim))
        {
            erro = "Os parâmetros dataInicio e dataFim são obrigatórios no formato YYYY-MM-DD.";
            return false;
        }

        var inicioOk = DateTime.TryParseExact(
            dataInicio,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var inicio);

        var fimOk = DateTime.TryParseExact(
            dataFim,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var fim);

        if (!inicioOk || !fimOk)
        {
            erro = "Datas inválidas. Use o formato YYYY-MM-DD.";
            return false;
        }

        if (fim < inicio)
        {
            erro = "dataFim não pode ser menor que dataInicio.";
            return false;
        }

        periodo = new PeriodoRelatorio(
            dataInicio!,
            dataFim!,
            inicio.Date,
            fim.Date.AddDays(1));

        return true;
    }

    private readonly record struct PeriodoRelatorio(
        string DataInicio,
        string DataFim,
        DateTime InicioUtc,
        DateTime FimExclusivoUtc);
}

public class VendasPeriodoResponse
{
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public decimal TotalVendas { get; set; }
    public int QuantidadeVendas { get; set; }
    public List<VendasPeriodoVendaResponse> Vendas { get; set; } = new();
}

public class VendasPeriodoVendaResponse
{
    public int Id { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public List<VendasPeriodoItemResponse> Itens { get; set; } = new();
}

public class VendasPeriodoItemResponse
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Subtotal { get; set; }
}

public class ProdutosEstoqueResponse
{
    public int TotalProdutos { get; set; }
    public int Criticos { get; set; }
    public int Normais { get; set; }
    public List<ProdutosEstoqueItemResponse> Produtos { get; set; } = new();
}

public class ProdutosEstoqueItemResponse
{
    public string Produto { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Atual { get; set; }
    public int Minimo { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class VendasPorClienteResponse
{
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public List<VendasPorClienteItemResponse> Clientes { get; set; } = new();
}

public class VendasPorClienteItemResponse
{
    public string Cliente { get; set; } = string.Empty;
    public int Pedidos { get; set; }
    public decimal Valor { get; set; }
    public decimal TicketMedio { get; set; }
}

public class ProdutosMaisVendidosResponse
{
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public List<ProdutosMaisVendidosItemResponse> Produtos { get; set; } = new();
}

public class ProdutosMaisVendidosItemResponse
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
