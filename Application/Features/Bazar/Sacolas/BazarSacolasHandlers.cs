using CRMVEXA.Application.Features.Bazar;
using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Services;
using CRMVEXA.Application.Services.Bazar;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Bazar.Sacolas;

public record GetSacolasConsignanteRequest(int ConsignanteId, bool IncluirResumo) : IRequest<List<BazarSacolaResponse>>;
public record GetSacolaRequest(int Id) : IRequest<BazarSacolaResponse>;

public class CreateSacolaRequest : IRequest<BazarSacolaResponse>
{
    public int ConsignanteId { get; set; }
    public string? Observacao { get; set; }
    public List<CreateSacolaPecaRequest> Pecas { get; set; } = new();
}

public class CreateSacolaPecaRequest
{
    public string Descricao { get; set; } = "";
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
    public decimal ValorCompra { get; set; }
    public decimal ValorVenda { get; set; }
}

public record DevolverSacolaRequest(int Id) : IRequest<BazarSacolaResponse>;

public class BazarSacolasHandlers :
    IRequestHandler<GetSacolasConsignanteRequest, List<BazarSacolaResponse>>,
    IRequestHandler<GetSacolaRequest, BazarSacolaResponse>,
    IRequestHandler<CreateSacolaRequest, BazarSacolaResponse>,
    IRequestHandler<DevolverSacolaRequest, BazarSacolaResponse>
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;

    public BazarSacolasHandlers(AppDbContext db, IEmpresaContext empresa)
    {
        _db = db;
        _empresa = empresa;
    }

    public async Task<List<BazarSacolaResponse>> Handle(GetSacolasConsignanteRequest request, CancellationToken ct)
    {
        await ValidarConsignanteAsync(request.ConsignanteId, ct);
        var sacolas = await _db.BazarSacolas.AsNoTracking()
            .Where(s => s.EmpresaId == _empresa.EmpresaId && s.ConsignanteId == request.ConsignanteId)
            .OrderByDescending(s => s.DataCadastro)
            .ThenByDescending(s => s.Id)
            .ToListAsync(ct);
        return await MapSacolasAsync(sacolas, request.IncluirResumo, ct);
    }

    public async Task<BazarSacolaResponse> Handle(GetSacolaRequest request, CancellationToken ct)
    {
        var sacola = await ObterSacolaAsync(request.Id, ct);
        var list = await MapSacolasAsync(new[] { sacola }, true, ct);
        return list[0];
    }

    public async Task<BazarSacolaResponse> Handle(CreateSacolaRequest request, CancellationToken ct)
    {
        if (request.Pecas == null || request.Pecas.Count == 0)
            throw BazarException.Validation("Informe ao menos uma peça.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var consignante = await ValidarConsignanteAsync(request.ConsignanteId, ct);
            var agora = DateTime.UtcNow;
            var hoje = DateTime.UtcNow.Date;
            var sacola = new BazarSacola
            {
                EmpresaId = _empresa.EmpresaId,
                ConsignanteId = consignante.Id,
                DataCadastro = hoje,
                Observacao = request.Observacao?.Trim(),
                CriadoEm = agora,
                AtualizadoEm = agora
            };
            _db.BazarSacolas.Add(sacola);
            await _db.SaveChangesAsync(ct);

            var pecasResp = new List<BazarPecaIdResponse>();
            foreach (var p in request.Pecas)
            {
                ValidarPeca(p.Descricao, p.ValorCompra, p.ValorVenda);
                var produto = new BazarProduto
                {
                    EmpresaId = _empresa.EmpresaId,
                    ConsignanteId = consignante.Id,
                    SacolaId = sacola.Id,
                    Descricao = p.Descricao.Trim(),
                    Tamanho = p.Tamanho?.Trim(),
                    Cor = p.Cor?.Trim(),
                    ValorCompra = p.ValorCompra,
                    ValorVenda = p.ValorVenda,
                    Status = BazarProdutoStatus.Disponivel,
                    DataEntrada = hoje,
                    CriadoEm = agora,
                    AtualizadoEm = agora
                };
                _db.BazarProdutos.Add(produto);
                await _db.SaveChangesAsync(ct);
                pecasResp.Add(new BazarPecaIdResponse { Id = produto.Id });
            }

            await tx.CommitAsync(ct);
            return new BazarSacolaResponse
            {
                Id = sacola.Id,
                ConsignanteId = sacola.ConsignanteId,
                DataCadastro = sacola.DataCadastro.ToString("yyyy-MM-dd"),
                Observacao = sacola.Observacao,
                Pecas = pecasResp
            };
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<BazarSacolaResponse> Handle(DevolverSacolaRequest request, CancellationToken ct)
    {
        var sacola = await ObterSacolaAsync(request.Id, ct);
        var pecas = await _db.BazarProdutos
            .Where(p => p.SacolaId == sacola.Id && p.EmpresaId == _empresa.EmpresaId && p.DeletadoEm == null)
            .ToListAsync(ct);

        if (pecas.Any(p => p.Status == BazarProdutoStatus.Vendida))
            throw BazarException.BusinessRule("Esta sacola tem peças vendidas. Devolva apenas as disponíveis individualmente.");

        var hoje = DateTime.UtcNow.Date;
        var agora = DateTime.UtcNow;
        foreach (var p in pecas.Where(p => p.Status == BazarProdutoStatus.Disponivel))
        {
            p.Status = BazarProdutoStatus.Devolvida;
            p.DataDevolucao = hoje;
            p.AtualizadoEm = agora;
        }
        sacola.AtualizadoEm = agora;
        await _db.SaveChangesAsync(ct);

        var list = await MapSacolasAsync(new[] { sacola }, true, ct);
        return list[0];
    }

    private async Task<BazarConsignante> ValidarConsignanteAsync(int consignanteId, CancellationToken ct)
    {
        var c = await _db.BazarConsignantes.FirstOrDefaultAsync(x =>
            x.Id == consignanteId && x.EmpresaId == _empresa.EmpresaId &&
            x.DeletadoEm == null && x.Ativo, ct);
        if (c == null) throw BazarException.NotFound("Consignante não encontrado ou inativo.");
        return c;
    }

    private async Task<BazarSacola> ObterSacolaAsync(int id, CancellationToken ct)
    {
        var s = await _db.BazarSacolas.FirstOrDefaultAsync(x =>
            x.Id == id && x.EmpresaId == _empresa.EmpresaId, ct);
        if (s == null) throw BazarException.NotFound("Sacola não encontrada.");
        return s;
    }

    private async Task<List<BazarSacolaResponse>> MapSacolasAsync(
        IEnumerable<BazarSacola> sacolas, bool incluirResumo, CancellationToken ct)
    {
        var lista = sacolas.ToList();
        if (lista.Count == 0) return new();

        var ids = lista.Select(s => s.Id).ToList();
        var produtos = await _db.BazarProdutos.AsNoTracking()
            .Where(p => ids.Contains(p.SacolaId) && p.EmpresaId == _empresa.EmpresaId && p.DeletadoEm == null)
            .Select(p => new { p.SacolaId, p.Status, p.ValorVenda, p.ValorCompra })
            .ToListAsync(ct);

        return lista.Select(s =>
        {
            var ps = produtos.Where(p => p.SacolaId == s.Id).ToList();
            var resp = new BazarSacolaResponse
            {
                Id = s.Id,
                ConsignanteId = s.ConsignanteId,
                DataCadastro = s.DataCadastro.ToString("yyyy-MM-dd"),
                Observacao = s.Observacao
            };
            if (!incluirResumo) return resp;
            var vendidas = ps.Count(p => p.Status == BazarProdutoStatus.Vendida);
            var total = ps.Count;
            resp.Total = total;
            resp.Disponiveis = ps.Count(p => p.Status == BazarProdutoStatus.Disponivel);
            resp.Vendidas = vendidas;
            resp.Devolvidas = ps.Count(p => p.Status == BazarProdutoStatus.Devolvida);
            resp.ValorVenda = ps.Sum(p => p.ValorVenda);
            resp.ValorRepasse = ps.Sum(p => p.ValorCompra);
            resp.PodeDevolverTudo = vendidas == 0 && total > 0;
            return resp;
        }).ToList();
    }

    internal static void ValidarPeca(string descricao, decimal valorCompra, decimal valorVenda)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Trim().Length > 200)
            throw BazarException.Validation("Descrição é obrigatória (máx. 200 caracteres).");
        if (valorCompra < 0 || valorVenda < 0)
            throw BazarException.Validation("Valores não podem ser negativos.");
        if (valorVenda < valorCompra)
            throw BazarException.BusinessRule("O valor de venda não pode ser menor que o valor de compra.");
    }
}
