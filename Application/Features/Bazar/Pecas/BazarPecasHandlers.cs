using CRMVEXA.Application.Features.Bazar;
using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Services;
using CRMVEXA.Application.Services.Bazar;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace CRMVEXA.Application.Features.Bazar.Pecas;

public record GetListaPecasRequest(
    int? ConsignanteId, int? SacolaId, string? Status,
    DateTime? DataInicio, DateTime? DataFim, int Pagina, int Tamanho) : IRequest<GetListaPecasResponse>;

public class GetListaPecasResponse
{
    public List<BazarPecaResponse> Pecas { get; set; } = new();
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}

public record GetPecaRequest(int Id) : IRequest<BazarPecaResponse>;

public class UpdatePecaRequest : IRequest<BazarPecaResponse>
{
    public int Id { get; set; }
    public string? Descricao { get; set; }
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
    public decimal? ValorCompra { get; set; }
    public decimal? ValorVenda { get; set; }
}

public record DevolverPecaRequest(int Id) : IRequest<BazarPecaResponse>;
public record DeletePecaRequest(int Id) : IRequest<Unit>;

public class UploadPecaFotosRequest : IRequest<UploadPecaFotosResponse>
{
    public int PecaId { get; set; }
    public IFormFileCollection Files { get; set; } = null!;
}

public class UploadPecaFotosResponse
{
    public List<BazarFotoResponse> Fotos { get; set; } = new();
}

public class BazarPecasHandlers :
    IRequestHandler<GetListaPecasRequest, GetListaPecasResponse>,
    IRequestHandler<GetPecaRequest, BazarPecaResponse>,
    IRequestHandler<UpdatePecaRequest, BazarPecaResponse>,
    IRequestHandler<DevolverPecaRequest, BazarPecaResponse>,
    IRequestHandler<DeletePecaRequest, Unit>,
    IRequestHandler<UploadPecaFotosRequest, UploadPecaFotosResponse>
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;
    private readonly BazarFotoStorageService _fotos;
    private readonly BazarAuditoriaService _auditoria;

    public BazarPecasHandlers(
        AppDbContext db,
        IEmpresaContext empresa,
        BazarFotoStorageService fotos,
        BazarAuditoriaService auditoria)
    {
        _db = db;
        _empresa = empresa;
        _fotos = fotos;
        _auditoria = auditoria;
    }

    public async Task<GetListaPecasResponse> Handle(GetListaPecasRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var query = _db.BazarProdutos.AsNoTracking()
            .Where(p => p.EmpresaId == empresaId && p.DeletadoEm == null);

        if (request.ConsignanteId.HasValue)
            query = query.Where(p => p.ConsignanteId == request.ConsignanteId.Value);
        if (request.SacolaId.HasValue)
            query = query.Where(p => p.SacolaId == request.SacolaId.Value);
        var status = BazarStatusHelper.FromApi(request.Status);
        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        if (request.DataInicio.HasValue)
            query = query.Where(p => p.DataEntrada >= request.DataInicio.Value.Date);
        if (request.DataFim.HasValue)
            query = query.Where(p => p.DataEntrada <= request.DataFim.Value.Date);

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho <= 0 ? 50 : request.Tamanho, 1, 500);
        var total = await query.CountAsync(ct);
        var totalPaginas = total > 0 ? (int)Math.Ceiling(total / (double)tamanho) : 0;

        var pecas = await query.OrderByDescending(p => p.DataEntrada).ThenByDescending(p => p.Id)
            .Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(ct);

        var fotos = await CarregarFotosAsync(pecas.Select(p => p.Id), ct);
        return new GetListaPecasResponse
        {
            Pecas = pecas.Select(p => Map(p, fotos.GetValueOrDefault(p.Id, new()))).ToList(),
            TotalPaginas = totalPaginas,
            TotalRegistros = total
        };
    }

    public async Task<BazarPecaResponse> Handle(GetPecaRequest request, CancellationToken ct)
    {
        var p = await ObterPecaAsync(request.Id, ct);
        var fotos = await CarregarFotosAsync(new[] { p.Id }, ct);
        return Map(p, fotos.GetValueOrDefault(p.Id, new()));
    }

    public async Task<BazarPecaResponse> Handle(UpdatePecaRequest request, CancellationToken ct)
    {
        var p = await ObterPecaAsync(request.Id, ct);
        if (p.Status != BazarProdutoStatus.Disponivel)
            throw BazarException.BusinessRule("Peça vendida ou devolvida não pode ser editada.");

        if (!string.IsNullOrWhiteSpace(request.Descricao)) p.Descricao = request.Descricao.Trim();
        if (request.Tamanho != null) p.Tamanho = request.Tamanho.Trim();
        if (request.Cor != null) p.Cor = request.Cor.Trim();
        if (request.ValorCompra.HasValue) p.ValorCompra = request.ValorCompra.Value;
        if (request.ValorVenda.HasValue) p.ValorVenda = request.ValorVenda.Value;
        Sacolas.BazarSacolasHandlers.ValidarPeca(p.Descricao, p.ValorCompra, p.ValorVenda);
        p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var fotos = await CarregarFotosAsync(new[] { p.Id }, ct);
        return Map(p, fotos.GetValueOrDefault(p.Id, new()));
    }

    public async Task<BazarPecaResponse> Handle(DevolverPecaRequest request, CancellationToken ct)
    {
        var p = await ObterPecaAsync(request.Id, ct);
        if (p.Status != BazarProdutoStatus.Disponivel)
            throw BazarException.BusinessRule("Apenas peças disponíveis podem ser devolvidas.");
        p.Status = BazarProdutoStatus.Devolvida;
        p.DataDevolucao = DateTime.UtcNow.Date;
        p.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var fotos = await CarregarFotosAsync(new[] { p.Id }, ct);
        return Map(p, fotos.GetValueOrDefault(p.Id, new()));
    }

    public async Task<Unit> Handle(DeletePecaRequest request, CancellationToken ct)
    {
        var p = await ObterPecaAsync(request.Id, ct);
        if (p.Status == BazarProdutoStatus.Vendida || p.VendaId.HasValue)
        {
            throw BazarException.ConflictWithCode(
                "PRODUCT_HAS_SALE",
                "Esta peça não pode ser excluída pois possui venda vinculada.");
        }

        var emRepasse = await _db.BazarRepasseItens.AnyAsync(ri => ri.ProdutoId == p.Id, ct);
        if (emRepasse)
            throw BazarException.BusinessRule("Peça já incluída em um repasse não pode ser excluída.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var agora = DateTime.UtcNow;
            p.DeletadoEm = agora;
            p.AtualizadoEm = agora;
            _auditoria.RegistrarExclusao("peca", p.Id, new { p.ConsignanteId, p.SacolaId, p.Status });
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }

        return Unit.Value;
    }

    public async Task<UploadPecaFotosResponse> Handle(UploadPecaFotosRequest request, CancellationToken ct)
    {
        var p = await ObterPecaAsync(request.PecaId, ct);
        if (request.Files == null || request.Files.Count == 0)
            throw BazarException.Validation("Envie ao menos uma imagem.");

        var existentes = await _db.BazarProdutoFotos.CountAsync(f => f.ProdutoId == p.Id, ct);
        if (existentes + request.Files.Count > 3)
            throw BazarException.Conflict("Máximo de 3 fotos por peça.");

        var ordemInicio = existentes;
        var salvas = new List<BazarFotoResponse>();
        for (var i = 0; i < request.Files.Count; i++)
        {
            var ordem = ordemInicio + i;
            if (ordem > 2) break;
            var url = await _fotos.SalvarFotoAsync(_empresa.EmpresaId, p.Id, ordem, request.Files[i], ct);
            var foto = new BazarProdutoFoto
            {
                ProdutoId = p.Id,
                Ordem = ordem,
                Url = url,
                CriadoEm = DateTime.UtcNow
            };
            _db.BazarProdutoFotos.Add(foto);
            await _db.SaveChangesAsync(ct);
            salvas.Add(new BazarFotoResponse { Id = foto.Id, Url = url, Ordem = ordem });
        }
        return new UploadPecaFotosResponse { Fotos = salvas };
    }

    private async Task<BazarProduto> ObterPecaAsync(int id, CancellationToken ct)
    {
        var p = await _db.BazarProdutos.FirstOrDefaultAsync(x =>
            x.Id == id && x.EmpresaId == _empresa.EmpresaId && x.DeletadoEm == null, ct);
        if (p == null) throw BazarException.NotFound("Peça não encontrada.");
        return p;
    }

    private async Task<Dictionary<int, List<BazarFotoResponse>>> CarregarFotosAsync(IEnumerable<int> pecaIds, CancellationToken ct)
    {
        var ids = pecaIds.Distinct().ToList();
        var rows = await _db.BazarProdutoFotos.AsNoTracking()
            .Where(f => ids.Contains(f.ProdutoId))
            .OrderBy(f => f.Ordem)
            .Select(f => new { f.ProdutoId, Foto = new BazarFotoResponse { Id = f.Id, Url = f.Url, Ordem = f.Ordem } })
            .ToListAsync(ct);
        return rows.GroupBy(x => x.ProdutoId).ToDictionary(g => g.Key, g => g.Select(x => x.Foto).ToList());
    }

    private static BazarPecaResponse Map(BazarProduto p, List<BazarFotoResponse> fotos) => new()
    {
        Id = p.Id,
        ConsignanteId = p.ConsignanteId,
        SacolaId = p.SacolaId,
        Descricao = p.Descricao,
        Tamanho = p.Tamanho,
        Cor = p.Cor,
        ValorCompra = p.ValorCompra,
        ValorVenda = p.ValorVenda,
        Fotos = fotos,
        Status = BazarStatusHelper.ToApi(p.Status),
        DataEntrada = p.DataEntrada.ToString("yyyy-MM-dd"),
        DataVenda = p.DataVenda?.ToString("yyyy-MM-dd"),
        DataDevolucao = p.DataDevolucao?.ToString("yyyy-MM-dd"),
        VendaId = p.VendaId
    };
}
