using System.Security.Claims;
using CRMVEXA.Application.Abstractions;
using CRMVEXA.Application.Constants;
using CRMVEXA.Application.Services;
using CRMVEXA.Application.Services.Personalizacao;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Personalizacao;

public record GetDevEmpresaSettingsRequest(int CompanyId) : IRequest<EmpresaSettingsResponse>;
public record UpdateDevEmpresaSettingsRequest(int CompanyId, AtualizarEmpresaSettingsRequest Body) : IRequest<EmpresaSettingsResponse>;
public record UploadDevEmpresaLogoRequest(int CompanyId, IFormFile File) : IRequest<LogoUploadResponse>;
public record DeleteDevEmpresaLogoRequest(int CompanyId) : IRequest<Unit>;
public record UpdateDevEmpresaModulosRequest(int CompanyId, AtualizarEmpresaModulosRequest Body) : IRequest<EmpresaSettingsResponse>;
public record GetMinhaEmpresaSettingsRequest : IRequest<EmpresaSettingsMeResponse>;

public class EmpresaPersonalizacaoHandlers :
    IRequestHandler<GetDevEmpresaSettingsRequest, EmpresaSettingsResponse>,
    IRequestHandler<UpdateDevEmpresaSettingsRequest, EmpresaSettingsResponse>,
    IRequestHandler<UploadDevEmpresaLogoRequest, LogoUploadResponse>,
    IRequestHandler<DeleteDevEmpresaLogoRequest, Unit>,
    IRequestHandler<UpdateDevEmpresaModulosRequest, EmpresaSettingsResponse>,
    IRequestHandler<GetMinhaEmpresaSettingsRequest, EmpresaSettingsMeResponse>
{
    private readonly ITenantAppDbContextFactory _tenantFactory;
    private readonly MasterDbContext _master;
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;
    private readonly EmpresaLogoStorageService _logos;
    private readonly IHttpContextAccessor _http;

    public EmpresaPersonalizacaoHandlers(
        ITenantAppDbContextFactory tenantFactory,
        MasterDbContext master,
        AppDbContext db,
        IEmpresaContext empresa,
        EmpresaLogoStorageService logos,
        IHttpContextAccessor http)
    {
        _tenantFactory = tenantFactory;
        _master = master;
        _db = db;
        _empresa = empresa;
        _logos = logos;
        _http = http;
    }

    public async Task<EmpresaSettingsResponse> Handle(GetDevEmpresaSettingsRequest request, CancellationToken ct)
    {
        await using var db = await CriarContextoTenantAsync(request.CompanyId, ct);
        return await MontarSettingsAsync(db, request.CompanyId, ct);
    }

    public async Task<EmpresaSettingsResponse> Handle(UpdateDevEmpresaSettingsRequest request, CancellationToken ct)
    {
        ValidarSettingsBody(request.Body);
        await using var db = await CriarContextoTenantAsync(request.CompanyId, ct);
        var config = await ObterOuCriarConfigAsync(db, request.CompanyId, ct);
        config.DisplayName = request.Body.DisplayName.Trim();
        config.PrimaryColor = request.Body.PrimaryColor;
        config.SecondaryColor = request.Body.SecondaryColor;
        config.DefaultTheme = ParseTema(request.Body.DefaultTheme);
        config.UpdatedAt = DateTime.UtcNow;
        config.UpdatedBy = ObterUsuarioId();
        await db.SaveChangesAsync(ct);
        return await MontarSettingsAsync(db, request.CompanyId, ct);
    }

    public async Task<LogoUploadResponse> Handle(UploadDevEmpresaLogoRequest request, CancellationToken ct)
    {
        await using var db = await CriarContextoTenantAsync(request.CompanyId, ct);
        var config = await ObterOuCriarConfigAsync(db, request.CompanyId, ct);
        _logos.RemoverSeExistir(config.LogoPath);
        var (path, filename) = await _logos.SalvarAsync(request.CompanyId, request.File, ct);
        config.LogoPath = path;
        config.LogoFilename = filename;
        config.UpdatedAt = DateTime.UtcNow;
        config.UpdatedBy = ObterUsuarioId();
        await db.SaveChangesAsync(ct);
        return new LogoUploadResponse { LogoUrl = path };
    }

    public async Task<Unit> Handle(DeleteDevEmpresaLogoRequest request, CancellationToken ct)
    {
        await using var db = await CriarContextoTenantAsync(request.CompanyId, ct);
        var config = await db.EmpresaConfiguracoes.FirstOrDefaultAsync(c => c.EmpresaId == request.CompanyId, ct);
        if (config == null)
            return Unit.Value;

        _logos.RemoverSeExistir(config.LogoPath);
        config.LogoPath = null;
        config.LogoFilename = null;
        config.UpdatedAt = DateTime.UtcNow;
        config.UpdatedBy = ObterUsuarioId();
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }

    public async Task<EmpresaSettingsResponse> Handle(UpdateDevEmpresaModulosRequest request, CancellationToken ct)
    {
        await using var db = await CriarContextoTenantAsync(request.CompanyId, ct);
        var estado = ConstruirEstadoModulos(request.Body.EnabledModules);
        ModuloCatalogo.ValidarModulos(estado);

        var agora = DateTime.UtcNow;
        var usuarioId = ObterUsuarioId();
        var rows = await db.EmpresaModulos.Where(m => m.EmpresaId == request.CompanyId).ToListAsync(ct);
        foreach (var def in ModuloCatalogo.Todos)
        {
            var enabled = estado[def.Key];
            var row = rows.FirstOrDefault(r => string.Equals(r.ModuleKey, def.Key, StringComparison.OrdinalIgnoreCase));
            if (row == null)
            {
                db.EmpresaModulos.Add(new EmpresaModulo
                {
                    EmpresaId = request.CompanyId,
                    ModuleKey = def.Key,
                    IsEnabled = enabled,
                    UpdatedAt = agora,
                    UpdatedBy = usuarioId
                });
            }
            else
            {
                row.IsEnabled = enabled;
                row.UpdatedAt = agora;
                row.UpdatedBy = usuarioId;
            }
        }

        await db.SaveChangesAsync(ct);
        return await MontarSettingsAsync(db, request.CompanyId, ct);
    }

    public async Task<EmpresaSettingsMeResponse> Handle(GetMinhaEmpresaSettingsRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var empresa = await _db.Empresas.AsNoTracking().FirstOrDefaultAsync(e => e.Id == empresaId, ct)
            ?? throw new InvalidOperationException("Empresa não encontrada.");

        await EmpresaPersonalizacaoSeed.GarantirPadraoAsync(_db, empresaId, empresa.Nome, ct: ct);

        var config = await _db.EmpresaConfiguracoes.AsNoTracking()
            .FirstAsync(c => c.EmpresaId == empresaId, ct);
        var modulos = await _db.EmpresaModulos.AsNoTracking()
            .Where(m => m.EmpresaId == empresaId)
            .ToListAsync(ct);
        var map = ModuloCatalogo.MesclarComPadrao(modulos.Select(m => new EmpresaModuloEstado(m.ModuleKey, m.IsEnabled)));

        return new EmpresaSettingsMeResponse
        {
            LogoUrl = config.LogoPath,
            DisplayName = config.DisplayName,
            PrimaryColor = config.PrimaryColor,
            SecondaryColor = config.SecondaryColor,
            DefaultTheme = config.DefaultTheme == TemaPadraoEmpresa.Light ? "light" : "dark",
            EnabledModules = map.Where(kv => kv.Value).Select(kv => kv.Key).OrderBy(k => k).ToList()
        };
    }

    private async Task<AppDbContext> CriarContextoTenantAsync(int companyId, CancellationToken ct)
    {
        var existe = await _master.Tenants.AsNoTracking()
            .AnyAsync(t => t.Id == companyId && t.Ativo, ct);
        if (!existe)
            throw new InvalidOperationException("Empresa não encontrada ou inativa.");
        return _tenantFactory.CreateDbContext(companyId);
    }

    private static async Task<EmpresaConfiguracao> ObterOuCriarConfigAsync(AppDbContext db, int empresaId, CancellationToken ct)
    {
        var config = await db.EmpresaConfiguracoes.FirstOrDefaultAsync(c => c.EmpresaId == empresaId, ct);
        if (config != null)
            return config;

        var nome = await db.Empresas.Where(e => e.Id == empresaId).Select(e => e.Nome).FirstOrDefaultAsync(ct)
            ?? "Empresa";
        await EmpresaPersonalizacaoSeed.GarantirPadraoAsync(db, empresaId, nome, ct: ct);
        return await db.EmpresaConfiguracoes.FirstAsync(c => c.EmpresaId == empresaId, ct);
    }

    private static async Task<EmpresaSettingsResponse> MontarSettingsAsync(AppDbContext db, int empresaId, CancellationToken ct)
    {
        var config = await ObterOuCriarConfigAsync(db, empresaId, ct);
        var modulos = await db.EmpresaModulos.AsNoTracking()
            .Where(m => m.EmpresaId == empresaId)
            .ToListAsync(ct);
        var map = ModuloCatalogo.MesclarComPadrao(modulos.Select(m => new EmpresaModuloEstado(m.ModuleKey, m.IsEnabled)));

        return new EmpresaSettingsResponse
        {
            LogoUrl = config.LogoPath,
            DisplayName = config.DisplayName,
            PrimaryColor = config.PrimaryColor,
            SecondaryColor = config.SecondaryColor,
            DefaultTheme = config.DefaultTheme == TemaPadraoEmpresa.Light ? "light" : "dark",
            EnabledModules = map.Where(kv => kv.Value).Select(kv => kv.Key).OrderBy(k => k).ToList()
        };
    }

    private static Dictionary<string, bool> ConstruirEstadoModulos(IEnumerable<string>? enabledKeys)
    {
        var estado = ModuloCatalogo.Todos.ToDictionary(m => m.Key, m => m.Fixed, StringComparer.OrdinalIgnoreCase);
        foreach (var key in enabledKeys ?? Enumerable.Empty<string>())
        {
            if (string.IsNullOrWhiteSpace(key)) continue;
            if (!ModuloCatalogo.TryObter(key, out _))
                throw new ModuloValidacaoException("UNKNOWN_MODULE", $"Módulo desconhecido: {key}.");
            estado[key] = true;
        }

        foreach (var def in ModuloCatalogo.Todos.Where(d => d.Fixed))
            estado[def.Key] = true;

        return estado;
    }

    private static void ValidarSettingsBody(AtualizarEmpresaSettingsRequest body)
    {
        if (string.IsNullOrWhiteSpace(body.DisplayName) || body.DisplayName.Trim().Length > 100)
            throw new InvalidOperationException("display_name é obrigatório (máx. 100 caracteres).");
        if (!ModuloCatalogo.EhHexValido(body.PrimaryColor))
            throw new InvalidOperationException("primary_color deve ser um hex válido (#RRGGBB).");
        if (!ModuloCatalogo.EhHexValido(body.SecondaryColor))
            throw new InvalidOperationException("secondary_color deve ser um hex válido (#RRGGBB).");
        var tema = ModuloCatalogo.NormalizarTema(body.DefaultTheme);
        if (tema is not ("dark" or "light"))
            throw new InvalidOperationException("default_theme deve ser 'dark' ou 'light'.");
    }

    private static TemaPadraoEmpresa ParseTema(string tema) =>
        ModuloCatalogo.NormalizarTema(tema) == "light" ? TemaPadraoEmpresa.Light : TemaPadraoEmpresa.Dark;

    private int? ObterUsuarioId()
    {
        var idStr = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) && id > 0 ? id : null;
    }
}
