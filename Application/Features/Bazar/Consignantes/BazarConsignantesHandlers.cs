using CRMVEXA.Application.Features.Bazar;
using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Services;
using CRMVEXA.Application.Services.Bazar;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Bazar.Consignantes;

public record GetListaConsignantesRequest(string? Busca, bool IncluirResumo, int Pagina, int Tamanho) : IRequest<GetListaConsignantesResponse>;
public class GetListaConsignantesResponse
{
    public List<BazarConsignanteResponse> Consignantes { get; set; } = new();
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}

public record GetConsignantesRecentesRequest(int Limite) : IRequest<List<BazarConsignanteResponse>>;
public record GetConsignantesResumoGeralRequest : IRequest<GetConsignantesResumoGeralResponse>;
public class GetConsignantesResumoGeralResponse
{
    public decimal PendenteTotal { get; set; }
    public int ConsignantesPendentes { get; set; }
    public int EmEstoque { get; set; }
    public int VendidoMes { get; set; }
}

public record GetConsignanteRequest(int Id) : IRequest<BazarConsignanteResponse>;
public record GetConsignanteResumoRequest(int Id) : IRequest<BazarResumoResponse>;

public class CreateConsignanteRequest : IRequest<BazarConsignanteResponse>
{
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Cpf { get; set; }
    public string? Observacao { get; set; }
}

public class UpdateConsignanteRequest : IRequest<BazarConsignanteResponse>
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }
    public string? Observacao { get; set; }
}

public record DeleteConsignanteRequest(int Id) : IRequest<Unit>;

public class BazarConsignantesHandlers :
    IRequestHandler<GetListaConsignantesRequest, GetListaConsignantesResponse>,
    IRequestHandler<GetConsignantesRecentesRequest, List<BazarConsignanteResponse>>,
    IRequestHandler<GetConsignantesResumoGeralRequest, GetConsignantesResumoGeralResponse>,
    IRequestHandler<GetConsignanteRequest, BazarConsignanteResponse>,
    IRequestHandler<GetConsignanteResumoRequest, BazarResumoResponse>,
    IRequestHandler<CreateConsignanteRequest, BazarConsignanteResponse>,
    IRequestHandler<UpdateConsignanteRequest, BazarConsignanteResponse>,
    IRequestHandler<DeleteConsignanteRequest, Unit>
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;
    private readonly BazarResumoService _resumo;

    public BazarConsignantesHandlers(AppDbContext db, IEmpresaContext empresa, BazarResumoService resumo)
    {
        _db = db;
        _empresa = empresa;
        _resumo = resumo;
    }

    public async Task<GetListaConsignantesResponse> Handle(GetListaConsignantesRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var query = _db.BazarConsignantes.AsNoTracking()
            .Where(c => c.EmpresaId == empresaId && c.DeletadoEm == null);

        if (!string.IsNullOrWhiteSpace(request.Busca))
        {
            var b = $"%{request.Busca.Trim()}%";
            query = query.Where(c => EF.Functions.ILike(c.Nome, b) || EF.Functions.ILike(c.Telefone, b));
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho <= 0 ? 50 : request.Tamanho, 1, 500);
        var total = await query.CountAsync(ct);
        var totalPaginas = total > 0 ? (int)Math.Ceiling(total / (double)tamanho) : 0;

        var items = await query.OrderBy(c => c.Nome)
            .Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(ct);

        Dictionary<int, int>? sacolasCount = null;
        Dictionary<int, BazarConsignanteResumoDto>? resumos = null;

        if (request.IncluirResumo && items.Count > 0)
        {
            var ids = items.Select(c => c.Id).ToList();
            sacolasCount = await _db.BazarSacolas.AsNoTracking()
                .Where(s => s.EmpresaId == empresaId && ids.Contains(s.ConsignanteId))
                .GroupBy(s => s.ConsignanteId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
            resumos = await _resumo.ObterResumosConsignantesAsync(empresaId, ids, ct);
        }

        return new GetListaConsignantesResponse
        {
            Consignantes = items.Select(c => Map(c, request.IncluirResumo, sacolasCount, resumos)).ToList(),
            TotalPaginas = totalPaginas,
            TotalRegistros = total
        };
    }

    public async Task<List<BazarConsignanteResponse>> Handle(GetConsignantesRecentesRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var limite = Math.Clamp(request.Limite <= 0 ? 6 : request.Limite, 1, 50);

        var recentes = await (
            from s in _db.BazarSacolas.AsNoTracking()
            join c in _db.BazarConsignantes.AsNoTracking() on s.ConsignanteId equals c.Id
            where s.EmpresaId == empresaId && c.DeletadoEm == null && c.Ativo
            group s by new { c.Id, c.Nome, c.Telefone } into g
            orderby g.Max(x => x.CriadoEm) descending
            select new BazarConsignanteResponse { Id = g.Key.Id, Nome = g.Key.Nome, Telefone = g.Key.Telefone }
        ).Take(limite).ToListAsync(ct);

        return recentes;
    }

    public async Task<GetConsignantesResumoGeralResponse> Handle(GetConsignantesResumoGeralRequest request, CancellationToken ct)
    {
        var (pendente, consPend, estoque, vendidoMes) = await _resumo.ObterResumoGeralAsync(_empresa.EmpresaId, ct);
        return new GetConsignantesResumoGeralResponse
        {
            PendenteTotal = pendente,
            ConsignantesPendentes = consPend,
            EmEstoque = estoque,
            VendidoMes = vendidoMes
        };
    }

    public async Task<BazarConsignanteResponse> Handle(GetConsignanteRequest request, CancellationToken ct)
    {
        var c = await ObterAtivoAsync(request.Id, ct);
        var resumo = await _resumo.ObterResumoConsignanteAsync(_empresa.EmpresaId, c.Id, ct);
        return Map(c, resumo);
    }

    public async Task<BazarResumoResponse> Handle(GetConsignanteResumoRequest request, CancellationToken ct)
    {
        await ObterAtivoAsync(request.Id, ct);
        var r = await _resumo.ObterResumoConsignanteAsync(_empresa.EmpresaId, request.Id, ct);
        return MapResumo(r);
    }

    public async Task<BazarConsignanteResponse> Handle(CreateConsignanteRequest request, CancellationToken ct)
    {
        ValidarConsignante(request.Nome, request.Telefone, request.Cpf);
        var empresaId = _empresa.EmpresaId;
        var cpfFmt = await NormalizarCpfUnicoAsync(empresaId, request.Cpf, null, ct);
        var agora = DateTime.UtcNow;
        var entity = new BazarConsignante
        {
            EmpresaId = empresaId,
            Nome = request.Nome.Trim(),
            Telefone = request.Telefone.Trim(),
            Cpf = cpfFmt,
            Observacao = request.Observacao?.Trim(),
            Ativo = true,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        _db.BazarConsignantes.Add(entity);
        await _db.SaveChangesAsync(ct);
        var resumo = await _resumo.ObterResumoConsignanteAsync(empresaId, entity.Id, ct);
        return Map(entity, resumo);
    }

    public async Task<BazarConsignanteResponse> Handle(UpdateConsignanteRequest request, CancellationToken ct)
    {
        var entity = await ObterAtivoAsync(request.Id, ct);
        if (!string.IsNullOrWhiteSpace(request.Nome)) entity.Nome = request.Nome.Trim();
        if (!string.IsNullOrWhiteSpace(request.Telefone)) entity.Telefone = request.Telefone.Trim();
        if (request.Observacao != null) entity.Observacao = request.Observacao.Trim();
        ValidarConsignante(entity.Nome, entity.Telefone, request.Cpf ?? entity.Cpf);
        entity.Cpf = await NormalizarCpfUnicoAsync(_empresa.EmpresaId, request.Cpf ?? entity.Cpf, entity.Id, ct);
        entity.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        var resumo = await _resumo.ObterResumoConsignanteAsync(_empresa.EmpresaId, entity.Id, ct);
        return Map(entity, resumo);
    }

    public async Task<Unit> Handle(DeleteConsignanteRequest request, CancellationToken ct)
    {
        var entity = await ObterAtivoAsync(request.Id, ct);
        var temPecasAtivas = await _db.BazarProdutos.AnyAsync(p =>
            p.ConsignanteId == entity.Id && p.DeletadoEm == null &&
            p.Status != BazarProdutoStatus.Devolvida, ct);
        if (temPecasAtivas)
            throw BazarException.Conflict("Não é possível excluir: existem peças que não foram todas devolvidas.");

        entity.DeletadoEm = DateTime.UtcNow;
        entity.Ativo = false;
        entity.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<BazarConsignante> ObterAtivoAsync(int id, CancellationToken ct)
    {
        var c = await _db.BazarConsignantes.FirstOrDefaultAsync(x =>
            x.Id == id && x.EmpresaId == _empresa.EmpresaId && x.DeletadoEm == null, ct);
        if (c == null) throw BazarException.NotFound("Consignante não encontrado.");
        return c;
    }

    private static void ValidarConsignante(string nome, string telefone, string? cpf)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length > 120)
            throw BazarException.Validation("Nome é obrigatório (máx. 120 caracteres).");
        var digitos = CpfValidator.ApenasDigitos(telefone);
        if (digitos == null || digitos.Length < 10)
            throw BazarException.Validation("Telefone é obrigatório (mínimo 10 dígitos).");
        if (!string.IsNullOrWhiteSpace(cpf) && !CpfValidator.EhValido(cpf))
            throw BazarException.Validation("CPF inválido.");
    }

    private async Task<string?> NormalizarCpfUnicoAsync(int empresaId, string? cpf, int? ignoreId, CancellationToken ct)
    {
        var digits = CpfValidator.ApenasDigitos(cpf);
        if (digits == null) return null;
        var fmt = CpfValidator.Formatar(digits);
        var existe = await _db.BazarConsignantes.AnyAsync(c =>
            c.EmpresaId == empresaId && c.Cpf == fmt && c.DeletadoEm == null &&
            (ignoreId == null || c.Id != ignoreId), ct);
        if (existe) throw BazarException.Conflict("Já existe um consignante com este CPF.");
        return fmt;
    }

    private static BazarConsignanteResponse Map(BazarConsignante c, bool incluirResumo,
        Dictionary<int, int>? sacolas, Dictionary<int, BazarConsignanteResumoDto>? resumos) =>
        new()
        {
            Id = c.Id,
            Nome = c.Nome,
            Telefone = c.Telefone,
            Cpf = c.Cpf,
            Observacao = c.Observacao,
            CriadoEm = c.CriadoEm.ToString("yyyy-MM-dd"),
            SacolasCount = incluirResumo ? sacolas?.GetValueOrDefault(c.Id) : null,
            Resumo = incluirResumo && resumos != null && resumos.TryGetValue(c.Id, out var r) ? MapResumo(r) : null
        };

    private static BazarConsignanteResponse Map(BazarConsignante c, BazarConsignanteResumoDto r) =>
        new()
        {
            Id = c.Id, Nome = c.Nome, Telefone = c.Telefone, Cpf = c.Cpf,
            Observacao = c.Observacao, CriadoEm = c.CriadoEm.ToString("yyyy-MM-dd"),
            Resumo = MapResumo(r)
        };

    private static BazarResumoResponse MapResumo(BazarConsignanteResumoDto r) => new()
    {
        Pendente = r.Pendente, EmEstoque = r.EmEstoque, Vendidas = r.Vendidas, Devolvidas = r.Devolvidas
    };
}
