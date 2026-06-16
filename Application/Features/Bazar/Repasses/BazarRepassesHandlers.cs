using System.Security.Claims;
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
using Npgsql;

namespace CRMVEXA.Application.Features.Bazar.Repasses;

public record GetRepassesPendentesRequest : IRequest<List<BazarRepassePendenteGrupoResponse>>;
public record GetRepassesPendentesConsignanteRequest(int ConsignanteId) : IRequest<List<BazarRepassePendenteGrupoResponse>>;

public record GetListaRepassesRequest(
    string? Status, int? ConsignanteId, DateTime? DataInicio, DateTime? DataFim, int Pagina, int Tamanho) : IRequest<GetListaRepassesResponse>;

public class GetListaRepassesResponse
{
    public List<BazarRepasseResponse> Repasses { get; set; } = new();
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}

public class CreateRepasseRequest : IRequest<BazarRepasseResponse>
{
    public int ConsignanteId { get; set; }
    public List<int> PecasIds { get; set; } = new();
    public string? FormaPagamento { get; set; }
    public string? Observacao { get; set; }
}

public class PagarRepasseRequest : IRequest<BazarRepasseResponse>
{
    public int Id { get; set; }
    public string? FormaPagamento { get; set; }
}

public record GetRepasseRequest(int Id) : IRequest<BazarRepasseResponse>;
public record DeleteRepasseRequest(int Id) : IRequest<Unit>;

public class BazarRepassesHandlers :
    IRequestHandler<GetRepassesPendentesRequest, List<BazarRepassePendenteGrupoResponse>>,
    IRequestHandler<GetRepassesPendentesConsignanteRequest, List<BazarRepassePendenteGrupoResponse>>,
    IRequestHandler<GetListaRepassesRequest, GetListaRepassesResponse>,
    IRequestHandler<CreateRepasseRequest, BazarRepasseResponse>,
    IRequestHandler<PagarRepasseRequest, BazarRepasseResponse>,
    IRequestHandler<GetRepasseRequest, BazarRepasseResponse>,
    IRequestHandler<DeleteRepasseRequest, Unit>
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;
    private readonly IHttpContextAccessor _http;

    public BazarRepassesHandlers(AppDbContext db, IEmpresaContext empresa, IHttpContextAccessor http)
    {
        _db = db;
        _empresa = empresa;
        _http = http;
    }

    public Task<List<BazarRepassePendenteGrupoResponse>> Handle(GetRepassesPendentesRequest request, CancellationToken ct) =>
        ListarPendentesAsync(null, ct);

    public Task<List<BazarRepassePendenteGrupoResponse>> Handle(GetRepassesPendentesConsignanteRequest request, CancellationToken ct) =>
        ListarPendentesAsync(request.ConsignanteId, ct);

    public async Task<GetListaRepassesResponse> Handle(GetListaRepassesRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var query = _db.BazarRepasses.AsNoTracking()
            .Include(r => r.Consignante)
            .Include(r => r.Itens)
            .Where(r => r.EmpresaId == empresaId && r.DeletadoEm == null);

        var st = BazarStatusHelper.RepasseFromApi(request.Status);
        if (st.HasValue) query = query.Where(r => r.Status == st.Value);
        if (request.ConsignanteId.HasValue)
            query = query.Where(r => r.ConsignanteId == request.ConsignanteId.Value);
        if (request.DataInicio.HasValue)
            query = query.Where(r => r.DataGeracao >= request.DataInicio.Value);
        if (request.DataFim.HasValue)
            query = query.Where(r => r.DataGeracao < request.DataFim.Value.AddDays(1));

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho <= 0 ? 50 : request.Tamanho, 1, 500);
        var total = await query.CountAsync(ct);
        var totalPaginas = total > 0 ? (int)Math.Ceiling(total / (double)tamanho) : 0;

        var items = await query.OrderByDescending(r => r.DataGeracao)
            .Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(ct);

        return new GetListaRepassesResponse
        {
            Repasses = items.Select(MapLista).ToList(),
            TotalPaginas = totalPaginas,
            TotalRegistros = total
        };
    }

    public async Task<BazarRepasseResponse> Handle(CreateRepasseRequest request, CancellationToken ct)
    {
        if (request.PecasIds == null || request.PecasIds.Count == 0)
            throw BazarException.Validation("Selecione ao menos uma peça.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var empresaId = _empresa.EmpresaId;
            var usuarioId = ObterUsuarioId();
            var pecas = await _db.BazarProdutos
                .Where(p => request.PecasIds.Contains(p.Id) && p.EmpresaId == empresaId && p.DeletadoEm == null)
                .ToListAsync(ct);

            if (pecas.Count != request.PecasIds.Distinct().Count())
                throw BazarException.NotFound("Uma ou mais peças não foram encontradas.");

            foreach (var p in pecas)
            {
                if (p.ConsignanteId != request.ConsignanteId)
                    throw BazarException.Validation("Todas as peças devem pertencer ao consignante informado.");
                if (p.Status != BazarProdutoStatus.Vendida)
                    throw BazarException.BusinessRule($"A peça '{p.Descricao}' não está vendida.");
            }

            var idsPecas = pecas.Select(p => p.Id).ToList();
            var itensAnteriores = await _db.BazarRepasseItens
                .Where(ri => idsPecas.Contains(ri.ProdutoId))
                .Join(_db.BazarRepasses, ri => ri.RepasseId, r => r.Id,
                    (ri, r) => new { Item = ri, r.DeletadoEm })
                .ToListAsync(ct);

            if (itensAnteriores.Any(x => x.DeletadoEm == null))
                throw BazarException.Conflict("Uma das peças já foi incluída em outro repasse.");

            // Itens órfãos de repasses cancelados precisam sair antes do insert:
            // o índice único em ProdutoId impediria incluir a peça em um novo repasse.
            if (itensAnteriores.Count > 0)
                _db.BazarRepasseItens.RemoveRange(itensAnteriores.Select(x => x.Item));

            var agora = DateTime.UtcNow;
            var repasse = new BazarRepasse
            {
                EmpresaId = empresaId,
                ConsignanteId = request.ConsignanteId,
                ValorTotal = pecas.Sum(p => p.ValorCompra),
                DataGeracao = agora,
                Status = BazarRepasseStatus.Pendente,
                FormaPagamento = request.FormaPagamento?.Trim(),
                Observacao = request.Observacao?.Trim(),
                CriadoPor = usuarioId,
                CriadoEm = agora,
                AtualizadoEm = agora
            };
            _db.BazarRepasses.Add(repasse);
            await _db.SaveChangesAsync(ct);

            foreach (var p in pecas)
            {
                _db.BazarRepasseItens.Add(new BazarRepasseItem
                {
                    RepasseId = repasse.Id,
                    ProdutoId = p.Id,
                    Valor = p.ValorCompra
                });
            }
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return await CarregarDetalheAsync(repasse.Id, ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            await tx.RollbackAsync(ct);
            throw BazarException.Conflict("Uma das peças já foi incluída em outro repasse.");
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<BazarRepasseResponse> Handle(PagarRepasseRequest request, CancellationToken ct)
    {
        var repasse = await ObterRepasseAsync(request.Id, ct);
        if (repasse.Status == BazarRepasseStatus.Pago)
            throw BazarException.BusinessRule("Repasse já está pago.");

        repasse.Status = BazarRepasseStatus.Pago;
        repasse.DataPagamento = DateTime.UtcNow;
        repasse.PagoPor = ObterUsuarioId();
        if (!string.IsNullOrWhiteSpace(request.FormaPagamento) && string.IsNullOrWhiteSpace(repasse.FormaPagamento))
            repasse.FormaPagamento = request.FormaPagamento.Trim();
        repasse.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await CarregarDetalheAsync(repasse.Id, ct);
    }

    public Task<BazarRepasseResponse> Handle(GetRepasseRequest request, CancellationToken ct) =>
        CarregarDetalheAsync(request.Id, ct);

    public async Task<Unit> Handle(DeleteRepasseRequest request, CancellationToken ct)
    {
        var repasse = await ObterRepasseAsync(request.Id, ct);
        if (repasse.Status == BazarRepasseStatus.Pago)
            throw BazarException.BusinessRule("Repasse pago não pode ser cancelado.");

        // Remove os itens para liberar as peças (índice único em ProdutoId);
        // sem isso as peças nunca mais poderiam entrar em outro repasse.
        var itens = await _db.BazarRepasseItens
            .Where(i => i.RepasseId == repasse.Id)
            .ToListAsync(ct);
        _db.BazarRepasseItens.RemoveRange(itens);

        repasse.DeletadoEm = DateTime.UtcNow;
        repasse.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<List<BazarRepassePendenteGrupoResponse>> ListarPendentesAsync(int? consignanteId, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var query = from p in _db.BazarProdutos.AsNoTracking()
                    join c in _db.BazarConsignantes.AsNoTracking() on p.ConsignanteId equals c.Id
                    where p.EmpresaId == empresaId && c.DeletadoEm == null && p.DeletadoEm == null
                          && p.Status == BazarProdutoStatus.Vendida
                          && !_db.BazarRepasseItens.Any(ri =>
                              ri.ProdutoId == p.Id &&
                              _db.BazarRepasses.Any(r => r.Id == ri.RepasseId && r.DeletadoEm == null))
                    select new { p, c };

        if (consignanteId.HasValue)
            query = query.Where(x => x.c.Id == consignanteId.Value);

        var rows = await query.ToListAsync(ct);
        return rows.GroupBy(x => new { x.c.Id, x.c.Nome, x.c.Telefone })
            .Select(g => new BazarRepassePendenteGrupoResponse
            {
                Consignante = new BazarRepasseConsignanteRef { Id = g.Key.Id, Nome = g.Key.Nome, Telefone = g.Key.Telefone },
                Pecas = g.Select(x => new BazarPecaPendenteResponse
                {
                    Id = x.p.Id,
                    Descricao = x.p.Descricao,
                    Tamanho = x.p.Tamanho,
                    Cor = x.p.Cor,
                    ValorCompra = x.p.ValorCompra,
                    DataVenda = x.p.DataVenda?.ToString("yyyy-MM-dd") ?? ""
                }).ToList(),
                Total = g.Sum(x => x.p.ValorCompra)
            })
            .Where(x => x.Total > 0)
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    private async Task<BazarRepasse> ObterRepasseAsync(int id, CancellationToken ct)
    {
        var r = await _db.BazarRepasses.FirstOrDefaultAsync(x =>
            x.Id == id && x.EmpresaId == _empresa.EmpresaId && x.DeletadoEm == null, ct);
        if (r == null) throw BazarException.NotFound("Repasse não encontrado.");
        return r;
    }

    private async Task<BazarRepasseResponse> CarregarDetalheAsync(int id, CancellationToken ct)
    {
        var r = await _db.BazarRepasses.AsNoTracking()
            .Include(x => x.Consignante)
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == _empresa.EmpresaId && x.DeletadoEm == null, ct);
        if (r == null) throw BazarException.NotFound("Repasse não encontrado.");

        var pecaIds = r.Itens.Select(i => i.ProdutoId).ToList();
        var pecas = await _db.BazarProdutos.AsNoTracking()
            .Where(p => pecaIds.Contains(p.Id))
            .ToListAsync(ct);

        return new BazarRepasseResponse
        {
            Id = r.Id,
            ConsignanteId = r.ConsignanteId,
            Consignante = new BazarRepasseConsignanteRef
            {
                Id = r.Consignante.Id,
                Nome = r.Consignante.Nome,
                Telefone = r.Consignante.Telefone
            },
            Data = r.DataGeracao.ToString("yyyy-MM-dd"),
            Valor = r.ValorTotal,
            FormaPagamento = r.FormaPagamento,
            Status = BazarStatusHelper.RepasseToApi(r.Status),
            PecasIds = pecaIds,
            Observacao = r.Observacao,
            Pecas = pecas.Select(p => new BazarPecaPendenteResponse
            {
                Id = p.Id,
                Descricao = p.Descricao,
                Tamanho = p.Tamanho,
                Cor = p.Cor,
                ValorCompra = p.ValorCompra,
                DataVenda = p.DataVenda?.ToString("yyyy-MM-dd") ?? ""
            }).ToList()
        };
    }

    private static BazarRepasseResponse MapLista(BazarRepasse r) => new()
    {
        Id = r.Id,
        ConsignanteId = r.ConsignanteId,
        Consignante = new BazarRepasseConsignanteRef
        {
            Id = r.Consignante.Id,
            Nome = r.Consignante.Nome,
            Telefone = r.Consignante.Telefone
        },
        Data = r.DataGeracao.ToString("yyyy-MM-dd"),
        Valor = r.ValorTotal,
        FormaPagamento = r.FormaPagamento,
        Status = BazarStatusHelper.RepasseToApi(r.Status),
        PecasIds = r.Itens.Select(i => i.ProdutoId).ToList(),
        Observacao = r.Observacao
    };

    private int ObterUsuarioId()
    {
        var idStr = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idStr, out var id) || id <= 0)
            throw new InvalidOperationException("Usuário não identificado.");
        return id;
    }
}
